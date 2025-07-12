import {Head, Link} from '@inertiajs/react';
import type {BreadcrumbItem} from '@/types';
import AppLayout from '@/layouts/app-layout';
import {Button} from '@/components/ui/button';
import InvoiceGrid from './Rechnungen/InvoiceGrid';

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

export default function App({primaryBusiness, invoices}: Props) {
    return (
        <AppLayout
            breadcrumbs={breadcrumbs}
            heading={"Welcome"}
            headerChildren={
                [primaryBusiness && (
                    <p className="text-muted-foreground text-lg">
                        Your primary business: <strong>{primaryBusiness.name}</strong>
                    </p>)]
            }
            leadingButtons={
                [
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Businesses">Businesses</Link>
                    </Button>,
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Businesses/Credit/Create">Create a credit</Link>
                    </Button>,
                    <Button asChild variant="secondary" size="lg">
                        <Link href="/Rechnungen/Create">Create Rechnung</Link>
                    </Button>
                ]
            }
        >
            <h2 className="text-2xl font-bold text-primary">Your Invoices</h2>
            <InvoiceGrid invoices={invoices}/>
        </AppLayout>
    );
}