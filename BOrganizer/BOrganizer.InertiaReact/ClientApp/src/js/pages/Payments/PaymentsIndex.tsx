import {Head, Link} from "@inertiajs/react";
import {columns} from "./Table/Columns";
import {PaymentDto} from "@/types/payment";
import AppLayout from "@/layouts/app-layout";
import {PageProps} from "@/types";

import {Button} from "@/components/ui/button"
import {DataTable} from "./Table/DataTable";
import {EarningsSummaryCard} from "@/pages/Payments/EarningsSummaryCard";
import {Business} from "@/types/busines";

interface Props extends PageProps {
    primaryBusiness?: Business;
    payments: PaymentDto[];
}

export default function Overview({payments, primaryBusiness,}: Props) {
    const totals = payments.reduce(
        (acc, p) => {
            if (p.receiverId != null && p.receiverId == primaryBusiness!.id
                || p.receiver == primaryBusiness!.name) {
                acc.netto += p.netto ?? 0;
                acc.brutto += p.brutto ?? 0;
                acc.mwst += (p.mwstEuro ?? 0);
            } else if (p.senderId != null && p.senderId == primaryBusiness!.id
                || p.sender == primaryBusiness!.name) {
                acc.netto -= p.netto ?? 0;
                acc.brutto -= p.brutto ?? 0;
                acc.mwst -= (p.mwstEuro ?? 0);
            }

            return acc;
        },
        {netto: 0, brutto: 0, mwst: 0}
    );

    return (
        <AppLayout
            heading="Payments"
            breadcrumbs={[{title: "Payments", href: "/Payments"}]}
            leadingButtons={[
                <Button asChild>
                    <Link href="/Payments/Create">Create Payment</Link>
                </Button>,
            ]}
        >
            <Head title="Payments"/>
            <EarningsSummaryCard
                netto={totals.netto}
                brutto={totals.brutto}
                mwst={totals.mwst}
            />
            <DataTable columns={columns} data={payments}/>
        </AppLayout>
    );
}