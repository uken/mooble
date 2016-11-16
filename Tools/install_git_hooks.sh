GIT_DIR=`git rev-parse --git-dir`

cat <<EOF > $GIT_DIR/hooks/pre-commit
#!/bin/bash
set -eo pipefail

echo Running stylecop
Tools/run_stylecop.sh
EOF

chmod +x $GIT_DIR/hooks/pre-commit
