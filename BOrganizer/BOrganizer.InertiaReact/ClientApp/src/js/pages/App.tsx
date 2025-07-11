import {Head, Link} from '@inertiajs/react';
import type {BreadcrumbItem} from "@/types";
import AppLayout from "@/layouts/app-layout";

type Business = {
    name: string;
};

type Invoice = {
    id: string;
    rechnungsnummer: string;
    erstellungsDatum: string; // ISO string
    rechnungsSteller: Business;
    rechnungsEmpfaenger: Business;
    gesamtBetrag: number;
};

type Props = {
    primaryBusiness: Business | null;
    invoices: Invoice[];
};

const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Dashboard',
        href: '/dashboard',
    },
];


export default function App({primaryBusiness, invoices}: Props) {
    return (
        <AppLayout breadcrumbs={breadcrumbs}>
            <Head title="Dashboard"/>
            <div className="flex h-full flex-1 flex-col gap-4 rounded-xl p-4 overflow-x-auto">
                <div
                    className="relative min-h-[100vh] flex-1 overflow-hidden rounded-xl border border-sidebar-border/70 md:min-h-min dark:border-sidebar-border">
                    <header className="mb-5">
                        <h1 className="display-4">Welcome</h1>
                    </header>

                    {primaryBusiness && (
                        <div className="mb-4">
                            <p className="lead">
                                Your primary business: <strong>{primaryBusiness.name}</strong>
                            </p>
                        </div>
                    )}

                    <div className="mb-5">
                        <Link className="btn btn-link me-3" href="/Businesses/BusinessesOverview">
                            Businesses
                        </Link>
                        <Link className="btn btn-link me-3" href="/Businesses/CreditCreation">
                            Create a credit
                        </Link>
                        <Link className="btn btn-link" href="/Rechnungen/RechnungCreation">
                            Create Rechnung
                        </Link>
                    </div>

                    <section>
                        <h2 className="mb-4">Your Invoices</h2>

                        <div className="row">
                            {invoices.map((invoice) => (
                                <div key={invoice.id} className="col-md-4 mb-4">
                                    <div className="card shadow-sm h-100">
                                        <div className="card-body d-flex flex-column">
                                            <h5 className="card-title mb-3">Rechnung {invoice.rechnungsnummer}</h5>

                                            <p className="mb-1">
                                                <strong>Am:</strong>{' '}
                                                {new Date(invoice.erstellungsDatum).toLocaleDateString('de-DE')}
                                            </p>
                                            <p className="mb-1">
                                                <strong>Steller:</strong> {invoice.rechnungsSteller.name}
                                            </p>
                                            <p className="mb-1">
                                                <strong>Empfänger:</strong> {invoice.rechnungsEmpfaenger.name}
                                            </p>
                                            <p className="mb-1 mt-auto">
                                                <strong>Summe:</strong>{' '}
                                                {invoice.gesamtBetrag.toLocaleString('de-DE', {
                                                    style: 'currency',
                                                    currency: 'EUR',
                                                })}
                                            </p>

                                            <Link
                                                href={`/Rechnungen/RechnungCreation?InvoiceId=${invoice.id}`}
                                                className="d-block mt-2"
                                            >
                                                Edit
                                            </Link>
                                            <Link href={`/Rechnungen/PdfView?InvoiceId=${invoice.id}`}
                                                  className="d-block">
                                                View Pdf
                                            </Link>
                                            <Link
                                                href={`/RechnungCreation/OnPostDownload?invoiceId=${invoice.id}`}
                                                className="d-block"
                                            >
                                                Download Pdf
                                            </Link>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </section>
                </div>
            </div>
        </AppLayout>
    );
}
