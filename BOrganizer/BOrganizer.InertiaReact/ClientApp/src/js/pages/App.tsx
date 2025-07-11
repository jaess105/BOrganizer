import { Head, Link } from '@inertiajs/react';
import type { BreadcrumbItem } from '@/types';
import AppLayout from '@/layouts/app-layout';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

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

export default function App({ primaryBusiness, invoices }: Props) {
    return (
        <AppLayout breadcrumbs={breadcrumbs}>
            <Head title="Dashboard" />

            <div className="flex flex-1 flex-col gap-6 p-6">
                <header className="space-y-1">
                    <h1 className="text-3xl font-bold tracking-tight">Welcome</h1>
                    {primaryBusiness && (
                        <p className="text-muted-foreground">
                            Your primary business: <strong>{primaryBusiness.name}</strong>
                        </p>
                    )}
                </header>

                <div className="flex flex-wrap gap-3">
                    <Button asChild variant="link">
                        <Link href="/Businesses/BusinessesOverview">Businesses</Link>
                    </Button>
                    <Button asChild variant="link">
                        <Link href="/Businesses/CreditCreation">Create a credit</Link>
                    </Button>
                    <Button asChild variant="link">
                        <Link href="/Rechnungen/RechnungCreation">Create Rechnung</Link>
                    </Button>
                </div>

                <section className="space-y-4">
                    <h2 className="text-2xl font-semibold">Your Invoices</h2>

                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {invoices.map((invoice) => (
                            <Card key={invoice.id} className="flex flex-col h-full">
                                <CardContent className="flex flex-col gap-2 p-4">
                                    <h3 className="text-lg font-medium">
                                        Rechnung {invoice.rechnungsnummer}
                                    </h3>

                                    <p>
                                        <span className="font-semibold">Am:</span>{' '}
                                        {new Date(invoice.erstellungsDatum).toLocaleDateString('de-DE')}
                                    </p>
                                    <p>
                                        <span className="font-semibold">Steller:</span>{' '}
                                        {invoice.rechnungsSteller.name}
                                    </p>
                                    <p>
                                        <span className="font-semibold">Empfänger:</span>{' '}
                                        {invoice.rechnungsEmpfaenger.name}
                                    </p>
                                    <p className="mt-auto">
                                        <span className="font-semibold">Summe:</span>{' '}
                                        {invoice.gesamtBetrag.toLocaleString('de-DE', {
                                            style: 'currency',
                                            currency: 'EUR',
                                        })}
                                    </p>

                                    <div className="mt-4 space-y-1">
                                        <Button asChild variant="outline" className="w-full">
                                            <Link href={`/Rechnungen/RechnungCreation?InvoiceId=${invoice.id}`}>
                                                Edit
                                            </Link>
                                        </Button>
                                        <Button asChild variant="outline" className="w-full">
                                            <Link href={`/Rechnungen/PdfView?InvoiceId=${invoice.id}`}>
                                                View Pdf
                                            </Link>
                                        </Button>
                                        <Button asChild variant="outline" className="w-full">
                                            <Link href={`/RechnungCreation/OnPostDownload?invoiceId=${invoice.id}`}>
                                                Download Pdf
                                            </Link>
                                        </Button>
                                    </div>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                </section>
            </div>
        </AppLayout>
    );
}
