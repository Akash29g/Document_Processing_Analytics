#!/usr/bin/env bash
set -euo pipefail

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
  local proj="$1"; local name="$2"; local dir="$OUT/$name"
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

find_cobertura () { find "$OUT/$1" -type f -name "coverage.cobertura.xml" | head -n 1; }

gen_summary () {
  local name="$1"; local cobertura="$2"; local target="$OUT/summary-$name"
  mkdir -p "$target"
  dotnet reportgenerator \
    "-reports:$cobertura" \
    "-targetdir:$target" \
    "-reporttypes:JsonSummary" \
    "-verbosity:Error" >/dev/null
}

for layer in domain data service api; do
  cov="$(find_cobertura "$layer")"
  if [[ -z "${cov:-}" ]]; then
    echo "ERROR: no coverage.cobertura.xml for $layer"; exit 2
  fi
  gen_summary "$layer" "$cov"
done

echo "== Coverage gate (per-assembly line %) =="
python3 - "$OUT" "$FLOOR_SERVICE" "$FLOOR_DOMAIN" "$FLOOR_DATA" "$FLOOR_API" <<'PY'
import json, os, sys
out = sys.argv[1]
targets = {
    "service": ("DocAnalytics.Service", float(sys.argv[2])),
    "domain":  ("DocAnalytics.Domain",  float(sys.argv[3])),
    "data":    ("DocAnalytics.Data",    float(sys.argv[4])),
    "api":     ("DocAnalytics.Api",     float(sys.argv[5])),
}

def asm_cov(report_name, asm_name):
    p = os.path.join(out, f"summary-{report_name}", "Summary.json")
    with open(p) as f:
        doc = json.load(f)
    for a in doc.get("coverage", {}).get("assemblies", []):
        if a.get("name") == asm_name:
            if a.get("coverage") is not None:
                return float(a["coverage"])
            cl = a.get("coveredlines", 0); cov = a.get("coverablelines", 0)
            return (100.0 * cl / cov) if cov else 0.0
    return None

fail = False
for rep, (asm, floor) in targets.items():
    lc = asm_cov(rep, asm)
    if lc is None:
        print(f"{rep:8} assembly {asm} NOT FOUND -> FAIL")
        fail = True
        continue
    ok = lc + 1e-9 >= floor
    if not ok:
        fail = True
    print(f"{rep:8} {asm:22} line={lc:6.2f}  floor={floor:5.1f}  -> {'OK' if ok else 'FAIL'}")
sys.exit(1 if fail else 0)
PY

