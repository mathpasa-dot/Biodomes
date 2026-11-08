# BioDomes

Application web de gestion de biodômes : création de biomes, gestion des espèces et des équipements, chaînes alimentaires (prédation), score d'équilibre d'un biome et tableau de bord avec historique.

Projet réalisé en 2e année de bachelier en informatique à HELMo (UE19 – Web avancé), en binôme avec **Mohammed Albashityalshaier**.

## Stack technique

- **ASP.NET Core 8**, Razor Pages, ASP.NET Core Identity (rôles Admin / Employee)
- **Entity Framework Core 8** + **SQLite**
- Architecture en trois projets : `BioDomes.Domains` (domaine), `BioDomes.Infrastructures` (EF Core, repositories), `BioDomes.Web` (UI)
- Chart.js et D3.js pour les graphiques du dashboard et le réseau trophique
- Docker, Docker Compose et Caddy (reverse proxy, HTTPS automatique)
- GitHub Actions : build, tests et publication de l'image sur GHCR

## Lancer en local

### Avec le SDK .NET 8

```bash
dotnet run --project BioDomes.Web
```

La base SQLite est créée automatiquement dans `BioDomes.Web/Data/` au premier démarrage (migrations appliquées au lancement).

### Avec Docker

```bash
cp .env.example .env        # puis adapter les mots de passe
docker compose up -d --build
```

Ouvrir https://localhost (certificat auto-signé de Caddy en local).

## Comptes de démonstration

Créés au démarrage si `SEED_DEMO_USERS=true` :

| Email | Rôle | Mot de passe |
|---|---|---|
| alice@company.com | Admin | `ADMIN_PASSWORD` (`.env`) |
| bob@company.com | Employee | `DEMO_PASSWORD` (`.env`) |
| celia@company.com | Employee | `DEMO_PASSWORD` (`.env`) |

En développement (sans variables d'environnement), le mot de passe est `Test1234$`.

## Configuration

Toute la configuration passe par des variables d'environnement (voir `.env.example`). Aucun secret n'est versionné.

| Variable | Rôle |
|---|---|
| `DOMAIN` | Domaine public du serveur (certificat Let's Encrypt via Caddy) |
| `BIODOMES_IMAGE` | Image à utiliser, par ex. `ghcr.io/<user>/biodomes:latest` |
| `ADMIN_PASSWORD` / `DEMO_PASSWORD` | Mots de passe des comptes de démonstration |
| `SMTP_*` | Serveur SMTP pour les emails de confirmation (sans SMTP, l'inscription est confirmée directement) |

Données persistées dans des volumes Docker :

- `biodomes-data` → `/app/data` (base `biodomes.sqlite` + clés de chiffrement des cookies)
- `biodomes-images` → `/app/wwwroot/images` (images fournies + uploads)

## Déploiement sur une VM Linux (exemple Azure)

Le déploiement ne dépend que de Docker : il fonctionne sur n'importe quel hébergeur (Azure, Hetzner, OVH, Scaleway…).

### 1. Créer la VM (Azure CLI)

```bash
az group create --name biodomes-rg --location westeurope

az vm create \
  --resource-group biodomes-rg \
  --name biodomes-vm \
  --image Ubuntu2404 \
  --size Standard_B1ms \
  --admin-username azureuser \
  --generate-ssh-keys \
  --public-ip-sku Standard \
  --public-ip-address-dns-name biodomes-<suffixe-unique>

az vm open-port --resource-group biodomes-rg --name biodomes-vm --port 80,443 --priority 1010
```

Le domaine obtenu est `biodomes-<suffixe-unique>.westeurope.cloudapp.azure.com`. Tu peux aussi utiliser ton propre domaine avec un enregistrement DNS `A` vers l'IP publique.

### 2. Installer Docker sur la VM

```bash
ssh azureuser@biodomes-<suffixe-unique>.westeurope.cloudapp.azure.com
curl -fsSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER && exit   # puis se reconnecter
```

### 3. Déployer

```bash
git clone https://github.com/<user>/biodomes.git && cd biodomes
cp .env.example .env && nano .env      # DOMAIN, mots de passe, (BIODOMES_IMAGE)
```

Puis, au choix :

- **Image publiée par GitHub Actions** (recommandé, rien à compiler sur la VM) :
  ```bash
  docker compose pull && docker compose up -d
  ```
  Après le premier passage de la CI, rendre le package public (GitHub → Packages → biodomes → Package settings → Change visibility), ou faire `docker login ghcr.io` sur la VM.
- **Build sur le serveur** (prévoir 2 Go de RAM) :
  ```bash
  docker compose up -d --build
  ```

### Mise à jour

```bash
git pull && docker compose pull && docker compose up -d
```

### Sauvegarde

```bash
docker compose cp app:/app/data/biodomes.sqlite ./backup-$(date +%F).sqlite
```

## Tests

```bash
dotnet test
```
