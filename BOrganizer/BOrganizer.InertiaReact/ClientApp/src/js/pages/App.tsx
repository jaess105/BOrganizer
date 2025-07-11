import { Head, Link } from '@inertiajs/react';
import type { BreadcrumbItem } from '@/types';
import AppLayout from '@/layouts/app-layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

type Business = {
    name: string;
};

type Invoice = {
    id: string;
    rechnungsnummer: string;
    erstellungsDatum: string;
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

            <div className="flex flex-1 flex-col gap-8 p-8 bg-background min-h-screen">
                <header className="space-y-2">
                    <h1 className="text-4xl font-extrabold tracking-tight text-primary">Welcome</h1>
                    {primaryBusiness && (
                        <p className="text-muted-foreground text-lg">
                            Your primary business: <strong>{primaryBusiness.name}</strong>
                        </p>
                    )}
                </header>

                <div className="flex flex-wrap gap-4">
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Businesses">Businesses</Link>
                    </Button>
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Businesses/Credit/Create">Create a credit</Link>
                    </Button>
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Rechnungen/RechnungCreation">Create Rechnung</Link>
                    </Button>
                </div>

                <section className="space-y-6">
                    <h2 className="text-2xl font-bold text-primary">Your Invoices</h2>

                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {invoices.map((invoice) => (
                            <Card key={invoice.id} className="flex flex-col h-full shadow-lg border border-muted">
                                <CardHeader className="pb-2">
                                    <h3 className="text-xl font-semibold text-primary">
                                        Rechnung {invoice.rechnungsnummer}
                                    </h3>
                                </CardHeader>
                                <CardContent className="flex flex-col gap-3 p-6">
                                    <div className="space-y-1">
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
                                    </div>
                                    <p className="mt-2 text-lg font-bold text-right text-success">
                                        {invoice.gesamtBetrag.toLocaleString('de-DE', {
                                            style: 'currency',
                                            currency: 'EUR',
                                        })}
                                    </p>
                                    <div className="mt-4 flex flex-col gap-2">
                                        <Button asChild variant="outline" size="sm">
                                            <Link href={`/Rechnungen/RechnungCreation?InvoiceId=${invoice.id}`}>
                                                Edit
                                            </Link>
                                        </Button>
                                        <Button asChild variant="outline" size="sm">
                                            <Link href={`/Rechnungen/PdfView?InvoiceId=${invoice.id}`}>
                                                View Pdf
                                            </Link>
                                        </Button>
                                        <Button asChild variant="outline" size="sm">
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