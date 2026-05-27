#!/bin/sh
set -eu

cat > /usr/share/nginx/html/env.js <<EOF
window.__env = window.__env || {};
window.__env.API_BASE_URL = "${API_BASE_URL:-}";
EOF