#!/usr/bin/env bash
set -euo pipefail

FLOOR_SERVICE=82
FLOOR_DOMAIN=72
FLOOR_DATA=90
FLOOR_API=44

ROOT="$(pwd)"
OUT="$ROOT/coverage-out"
rm -rf "$OUT"; mkdir -p "$OUT"

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
    --results-directory "$dir"
}

run_test "DocAnalytics.Domain.Tests/DocAnalytics.Domain.Tests.csproj"   "domain"
run_test "DocAnalytics.Data.Tests/DocAnalytics.Data.Tests.csproj"       "data"
run_test "DocAnalytics.Service.Tests/DocAnalytics.Service.Tests.csproj" "service"
run_test "DocAnalytics.Api.Tests/DocAnalytics.Api.Tests.csproj"         "api"

mapfile -t COBS < <(find "$OUT" -type f -name "coverage.cobertura.xml")
if [[ ${#COBS[@]} -eq 0 ]]; then
  echo "ERROR: no cobertura files found"; exit 2
fi
REPORTS=$(IFS=';'; echo "${COBS[*]}")
echo "== Merging ${#COBS[@]} coverage files into one report =="

dotnet reportgenerator \
  "-reports:$REPORTS" \
  "-targetdir:$OUT/summary" \
  "-reporttypes:JsonSummary" \
  "-verbosity:Error" >/dev/null

echo "== Coverage gate (per-assembly line %) =="
python3 - "$OUT/summary/Summary.json" "$FLOOR_SERVICE" "$FLOOR_DOMAIN" "$FLOOR_DATA" "$FLOOR_API" <<'PY'
import json, sys
with open(sys.argv[1]) as f:
    doc = json.load(f)
floors = {
    "DocAnalytics.Service": float(sys.argv[2]),
    "DocAnalytics.Domain":  float(sys.argv[3]),
    "DocAnalytics.Data":    float(sys.argv[4]),
    "DocAnalytics.Api":     float(sys.argv[5]),
}
asms = { a["name"]: a for a in doc.get("coverage", {}).get("assemblies", []) }
fail = False
for name, floor in floors.items():
    a = asms.get(name)
    if a is None:
        print(f"{name:22} NOT FOUND -> FAIL"); fail = True; continue
    lc = float(a["coverage"]) if a.get("coverage") is not None else \
         (100.0 * a.get("coveredlines", 0) / a["coverablelines"] if a.get("coverablelines") else 0.0)
    ok = lc + 1e-9 >= floor
    if not ok: fail = True
    print(f"{name:22} line={lc:6.2f}  floor={floor:5.1f}  -> {'OK' if ok else 'FAIL'}")
sys.exit(1 if fail else 0)
PY
