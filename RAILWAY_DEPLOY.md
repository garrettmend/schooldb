# Railway deployment notes

- This repo contains a Dockerfile at `LMS/Dockerfile` to build a container for the app.
- The app reads the `MYSQL_URL` environment variable (format: `mysql://user:pass@host:port/db`) and uses it for both the application DB (`Team19LMSContext`) and Identity (`IdentityConnection`) when present.
- For Railway, add an environment variable named `MYSQL_URL` with the value:

  `mysql://root:DfGxDgpWqieOMDuizbkDgFDIaHAJycGq@mysql.railway.internal:3306/railway`

- The app no longer requires a local PFX at deploy time. If you do want the app to serve TLS itself, set these environment variables in Railway:
  - `HTTPS_PFX_PATH` — path to a PFX file inside the container (e.g., `/run/secrets/https.pfx`)
  - `HTTPS_PFX_PASSWORD` — password for the PFX

  If `HTTPS_PFX_PATH` is absent or the file is missing, the app will only listen on the port Railway provides (Railway performs TLS termination in front of the container).

- Persist Data Protection keys so antiforgery tokens, cookies, and other protected data survive restarts:
  - Set the env var `DATA_PROTECTION_PATH` to a directory (for example `/var/keys`).
  - Mount a persistent volume at that path (Railway supports persistent volumes) so keys are shared across restarts and replicas.
  - Example Docker Compose (local testing) uses `/var/keys` and a named volume `dp_keys`.

- If you want the app to perform HTTPS redirects inside the container, set `ENFORCE_HTTPS=true` in your Railway service; otherwise leave it unset so the app will not attempt to redirect to an HTTPS port it doesn't manage.
- A `docker-compose.yml` is included for local testing; it sets `MYSQL_URL` the same way by default.

Notes:
- No application logic or authentication changes were made. This only prepares the app for deployment on Railway using Docker.
