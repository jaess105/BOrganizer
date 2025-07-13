import React from 'react';
import {Card, CardContent, CardHeader} from '@/components/ui/card';
import {Button} from '@/components/ui/button';
import {Link} from '@inertiajs/react';
import {Invoice} from '@/types/busines';

type InvoiceGridProps = {
    invoices: Invoice[];
};

const InvoiceGrid: React.FC<InvoiceGridProps> = ({invoices}) => (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {invoices.map((invoice) => (
            <Card key={invoice.id} className="flex flex-col h-full shadow-lg border border-muted">
                <CardHeader className="pb-2">
                    <h3 className="text-xl font-semibold text-primary">
                        Rechnung {invoice.rechnungsnummer.toString()}
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
                            <Link href={`/Rechnungen/Create?invoiceId=${invoice.id}`}>
                                Edit
                            </Link>
                        </Button>
                        <Button asChild variant="outline" size="sm">
                            <Link href={`/Rechnungen/Pdf/View?invoiceId=${invoice.id}`}>
                                View Pdf
                            </Link>
                        </Button>
                        <Button asChild variant="outline" size="sm">
                            <Link href={`/Payments/Create/FromInvoice/${invoice.id}`}>
                                To Payment
                            </Link>
                        </Button>
                    </div>
                </CardContent>
            </Card>
        ))}
    </div>
);

export default InvoiceGrid;