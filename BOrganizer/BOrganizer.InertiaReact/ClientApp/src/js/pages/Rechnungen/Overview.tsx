import {Head, Link} from '@inertiajs/react';
import {type BreadcrumbItem, PageProps} from '@/types';
import {Button} from '@/components/ui/button';
import AppLayout from "@/layouts/app-layout";
import {Invoice} from "@/types/busines";
import {InvoiceGrid} from './InvoiceGrid';
import {Payment} from "@/types/payment";

interface Props extends PageProps {
    invoices: Invoice[],
    payments?: Record<number, Payment>,
}

const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Rechnungen',
        href: '/Rechnungen',
    },
];

export default function RechnungenOverview({invoices, payments}: Props) {
    return (
        <AppLayout
            breadcrumbs={breadcrumbs}
            heading={"All Invoices"}
            leadingButtons={[
                <Button asChild variant="secondary" size="lg">
                    <Link href="/Rechnungen/Create">Create an Invoice</Link>
                </Button>
            ]}
        >
            <Head title="Rechnungen"/>
            <InvoiceGrid invoices={invoices} payments={payments} />
        </AppLayout>
    );
}
