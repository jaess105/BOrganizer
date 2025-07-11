import {Head, Link} from '@inertiajs/react';
import type {BreadcrumbItem} from '@/types';
import AppLayout from '@/layouts/app-layout';
import {Card, CardContent, CardHeader} from '@/components/ui/card';
import {Button} from '@/components/ui/button';
import {Invoice} from "@/types/busines";
import RechnungPdfFrame from "@/pages/Rechnungen/RechnungPdfFrame";


const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Dashboard',
        href: '/dashboard/Rechnung',
    },
];

type Props = {
    invoice: Invoice;
};

export default function RechnungPdfView({invoice}: Props) {
    return (
        <AppLayout breadcrumbs={breadcrumbs}>
            <Head title="Dashboard"/>
            <div className="space-y-8 p-6">
                <RechnungPdfFrame invoiceId={invoice.id!} alwaysShow={true}/>
            </div>
        </AppLayout>
    );
}