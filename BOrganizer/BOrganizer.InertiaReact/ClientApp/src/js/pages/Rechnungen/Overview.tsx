import {Head, Link} from '@inertiajs/react';
import {type BreadcrumbItem, PageProps} from '@/types';
import {Button} from '@/components/ui/button';
import AppLayout from "@/layouts/app-layout";
import {Invoice} from "@/types/busines";
import InvoiceGrid from './InvoiceGrid';

interface Props extends PageProps {
    invoices: Invoice[]
}

const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Rechnungen',
        href: '/Rechnungen',
    },
];

export default function RechnungenOverview({invoices}: Props) {
    return (
        <AppLayout
            breadcrumbs={breadcrumbs}
            heading={"All Invoices"}
            leadingButtons={[
                <Button asChild variant="secondary" size="lg">
                    <Link href="/Rechnungen/Create">Create a Invoice</Link>
                </Button>
            ]}
        >
            <Head title="Rechnungen"/>
            <InvoiceGrid invoices={invoices.map(function (i) {
                return {
                    id: i.id!.toString(),
                    rechnungsnummer: i.rechnungsnummer.toString(),
                    erstellungsDatum: i.erstellungsDatum.toString(),
                    rechnungsSteller: i.rechnungsSteller,
                    rechnungsEmpfaenger: i.rechnungsEmpfaenger,
                    gesamtBetrag: i.gesamtBetrag,
                };
            })}/>
        </AppLayout>
    );
}
