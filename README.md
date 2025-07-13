# BOrganizer

Simple small business organizer. Currently does not support many operations.
But is capable of creating invoices for different companies after the german standard.

The main goal of the project is not public use.
It is just a small hobby project.
But anyone interested can use it.

## Usage

Simply build and run the docker compose.

```sh
docker compose up --build
```

or use the run (bash) script which supports the `dev` and `compose` commands.

## Technology

The current solution uses [Inertia.js](https://inertiajs.com/) via [InertiaCore](https://github.com/kapi2289/InertiaCore)
which enables the serving of react components from a dotnet backend.
This makes it easy to deliver nice reactive components while using a trusted backend stack such as SP.NET Core.

The project was started as a simple razor pages application but this will not be developed further.
The last version is kept on [this branch](https://github.com/jaess105/BOrganizer/tree/pages-version).

The Rechnungen.Creator.PDF uses [QuestPDF](https://www.questpdf.com/) for PDF generation.
It uses the community license as this is a FOSS. 
The project can simply be swapped out as long as the new creator class implements `IRechnungsCreator`.   

## Todos

- [x] invoice download
- [x] invoice details page

### future

- [ ] PDF customization
