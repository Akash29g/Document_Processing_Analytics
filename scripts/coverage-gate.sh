#!/usr/bin/env bash
set -euo pipefail

# Floors (baseline - 2%). Keep synced with docs/coverage-baseline.md.
FLOOR_SERVICE=79
FLOOR_DOMAIN=75
FLOOR_DATA=23
FLOOR_API=22

ROOT="$(pwd)"
OUT="$ROOT/coverage-out"
mkdir -p "$OUT"

echo "== Restore local tools (ReportGenerator) =="
dotnet tool restore

run_test () {
  local proj="$1"
  local name="$2"
  local dir="$OUT/$name"
  mkdir -p "$dir"

  echo "== dotnet test: $name =="
  dotnet test "$proj" \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --settings coverage.runsettings \
    --results-directory "$dir" \
    --logger "trx;LogFileName=$name.trx"
}

run_test "DocAnalytics.Domain.Tests/DocAnalytics.Domain.Tests.csproj"   "domain"
run_test "DocAnalytics.Data.Tests/DocAnalytics.Data.Tests.csproj"       "data"
run_test "DocAnalytics.Service.Tests/DocAnalytics.Service.Tests.csproj" "service"
run_test "DocAnalytics.Api.Tests/DocAnalytics.Api.Tests.csproj"         "api"

find_cobertura () {
  local name="$1"
  find "$OUT/$name" -type f -name "coverage.cobertura.xml" | head -n 1
}

cov_domain="$(find_cobertura domain)"
cov_data="$(find_cobertura data)"
cov_service="$(find_cobertura service)"
cov_api="$(find_cobertura api)"

if [[ -z "${cov_domain:-}" || -z "${cov_data:-}" || -z "${cov_service:-}" || -z "${cov_api:-}" ]]; then
  echo "ERROR: Could not find one or more coverage.cobertura.xml files."
  exit 2
fi

echo "== ReportGenerator: JSON summary per layer =="

gen_summary () {
  local name="$1"
  local cobertura="$2"
  local target="$OUT/summary-$name"
  mkdir -p "$target"

  dotnet reportgenerator \
    "-reports:$cobertura" \
    "-targetdir:$target" \
    "-reporttypes:JsonSummary"

  cat "$target/Summary.json"
}

sum_domain="$(gen_summary domain "$cov_domain")"
sum_data="$(gen_summary data "$cov_data")"
sum_service="$(gen_summary service "$cov_service")"
sum_api="$(gen_summary api "$cov_api")"

extract_line () {
  local json="$1"
  echo "$json" | python3 - <<'PY'
import json, sys
doc = json.load(sys.stdin)
print(doc["summary"]["linecoverage"])
PY
}

lc_domain="$(extract_line "$sum_domain")"
lc_data="$(extract_line "$sum_data")"
lc_service="$(extract_line "$sum_service")"
lc_api="$(extract_line "$sum_api")"

echo "== Coverage (line %) =="
printf "Service: %s (floor %s)\n" "$lc_service" "$FLOOR_SERVICE"
printf "Domain : %s (floor %s)\n" "$lc_domain"  "$FLOOR_DOMAIN"
printf "Data   : %s (floor %s)\n" "$lc_data"    "$FLOOR_DATA"
printf "Api    : %s (floor %s)\n" "$lc_api"     "$FLOOR_API"

fail=0
python3 - <<PY || fail=1
import sys
def below(val, floor): return float(val) + 1e-9 < float(floor)
lc_service=float("$lc_service"); lc_domain=float("$lc_domain"); lc_data=float("$lc_data"); lc_api=float("$lc_api")
if below(lc_service, $FLOOR_SERVICE): print("FAIL: Service below floor"); sys.exit(1)
if below(lc_domain,  $FLOOR_DOMAIN):  print("FAIL: Domain below floor");  sys.exit(1)
if below(lc_data,    $FLOOR_DATA):    print("FAIL: Data below floor");    sys.exit(1)
if below(lc_api,     $FLOOR_API):     print("FAIL: Api below floor");     sys.exit(1)
print("PASS: Coverage floors met")
PY

exit $fail
