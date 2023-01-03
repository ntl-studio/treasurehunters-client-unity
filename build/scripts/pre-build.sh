#!/bin/bash
echo "[npmAuth.'https://npm.pkg.github.com/@ntl-studio']" >> ~/.upmconfig.toml
echo "token = '$NPM_TOKEN'" >> ~/.upmconfig.toml
echo "email = '$NPM_TOKEN_EMAIL'" >> ~/.upmconfig.toml
echo "alwaysAuth = true" >> ~/.upmconfig.tom
