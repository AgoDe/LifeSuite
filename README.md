# LifeSuite

**LifeSuite** è una piattaforma modulare per la gestione della vita quotidiana, basata su architettura a microservizi e orchestrata tramite un API Gateway sviluppato in Node.js (AdonisJS).

## Caratteristiche principali

- **API Gateway centralizzato** (AdonisJS/Node.js): punto di accesso unico per frontend web/mobile.
- **Gestione Budget personale** tramite il microservizio _BudgetManager_ (.NET), con tracciamento di transazioni, reportistica e analisi delle spese.
- **Gestione Piano Alimentare** tramite il microservizio _MealPlanner_ (Laravel), con creazione di diete, pasti e tracking nutrizionale.
- **Autenticazione sicura** e propagazione delle identità utente tra i servizi tramite JWT.
- **Scalabilità**: aggiunta semplice di nuovi microservizi (es: fitness, notifiche, analytics).
- **Architettura moderna**: separazione delle responsabilità, sviluppi paralleli, deploy indipendenti.
- **Comunicazione tra servizi** tramite API interne e message broker (RabbitMQ, Kafka - opzionali).

## Architettura

```
[Frontend (Vue3/mobile)]
        |
        v
[API Gateway (AdonisJS)]
   |                  |
   v                  v
[BudgetManager (.NET)]    [MealPlanner (Laravel)]
```

## Come iniziare

1. Clona la repository.
2. Consulta la documentazione delle cartelle `gateway/`, `budgetmanager/`, `mealplanner/`.
3. Avvia i servizi tramite Docker Compose:  
   ```bash
   docker-compose up --build
   ```
4. Accedi al Gateway su `http://localhost:3000`.

## Tecnologie principali

- Node.js (AdonisJS) – API Gateway
- .NET 7/8 – BudgetManager
- Laravel 10 – MealPlanner
- Docker, Docker Compose
- JWT per autenticazione/propagazione utente

## Contribuire

Proponi nuove feature, segnala bug o chiedi supporto tramite le [issue](../../issues).

---

> **LifeSuite** – Organizza, pianifica, semplifica la tua vita.
