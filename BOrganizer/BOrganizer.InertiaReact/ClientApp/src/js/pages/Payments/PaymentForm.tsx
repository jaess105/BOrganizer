import {useEffect, useState} from 'react';
import {router, useForm} from '@inertiajs/react';
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue,} from '@/components/ui/select';
import {Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle,} from '@/components/ui/dialog';
import {Card, CardContent, CardFooter, CardHeader, CardTitle,} from '@/components/ui/card';
import AppLayout from '@/layouts/app-layout';
import type {BreadcrumbItem, PageProps} from '@/types';
import {PaymentDto} from '@/types/payment';
import {format} from 'date-fns';
import {Label} from '@/components/ui/label';
import {Input} from "@/components/ui/input";
import {Textarea} from '@/components/ui/textarea';
import {Button} from "@/components/ui/button";
import {Command, CommandEmpty, CommandInput, CommandItem, CommandList,} from "@/components/ui/command"

import {Popover, PopoverContent, PopoverTrigger} from "@/components/ui/popover";
import {cn} from "@/lib/utils"; // assuming utility exists
import {Check} from "lucide-react";
import axios from "axios";


interface Props extends PageProps {
    payment?: PaymentDto | null;
}

const breadcrumbs: BreadcrumbItem[] = [
    {title: 'Payments', href: '/Payments'},
    {title: 'Create Payment', href: '/Payments/Create'},
];

export default function CreatePaymentPage({payment}: Props) {
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const {data, setData, post, processing, errors} = useForm({
        id: payment?.paymentId,
        date: payment?.date ?? format(new Date(), 'yyyy-MM-dd'),
        netto: payment?.netto ?? 0,
        mwstPercent: payment?.mwstPercent ?? 19,
        mwstEuro: payment?.mwstEuro ?? 0,
        brutto: payment?.brutto ?? 0,
        sender: payment?.sender ?? '',
        receiver: payment?.receiver ?? '',
        product: payment?.product ?? '',
        currency: payment?.currency ?? 'EUR',
        method: payment?.method ?? 'Überweisung',
        notes: payment?.notes ?? '',
        rechnungId: payment?.rechnungId ?? undefined,
        senderId: payment?.senderId,
        receiverId: payment?.receiverId,
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        post('/Payments/Create', {
            forceFormData: true,
            onSuccess: () => setShowSuccessModal(true),
        });
    };

    const [lastChanged, setLastChanged] = useState<"netto" | "brutto" | "mwstPercent" | null>(null);

    useEffect(() => {
        if (lastChanged === "netto" || lastChanged === "mwstPercent") {
            // Calculate brutto and mwstEuro from netto and mwstPercent
            const mwst = data.netto * (data.mwstPercent / 100);
            const brutto = data.netto + mwst;

            setData((prev) => ({
                ...prev,
                brutto: parseFloat(brutto.toFixed(2)),
                mwstEuro: parseFloat(mwst.toFixed(2)),
            }));
        } else if (lastChanged === "brutto") {
            // Calculate netto and mwstEuro from brutto and mwstPercent
            const netto = data.brutto / (1 + data.mwstPercent / 100);
            const mwst = data.brutto - netto;

            setData((prev) => ({
                ...prev,
                netto: parseFloat(netto.toFixed(2)),
                mwstEuro: parseFloat(mwst.toFixed(2)),
            }));
        }
    }, [data.netto, data.brutto, data.mwstPercent, lastChanged]);


    const [searchOpen, setSearchOpen] = useState(false);
    const [invoiceOptions, setInvoiceOptions] = useState<{ id: number, label: string }[]>([]);
    const handleSearch = async (query: string) => {
        if (!query) return;

        const res = await axios.get('/Api/Rechnung/Search', {params: {q: query}});
        setInvoiceOptions(res.data);
    };


    return (
        <AppLayout breadcrumbs={breadcrumbs} heading="Create Payment">
            <form onSubmit={handleSubmit} className="max-w-3xl mx-auto space-y-6 px-4 py-8">
                <Card>
                    <CardHeader>
                        <CardTitle>Payment Details</CardTitle>
                    </CardHeader>

                    <CardContent className="space-y-6">
                        {/* Date */}
                        <div>
                            <Label htmlFor="date">Date</Label>
                            <Input
                                id="date"
                                type="date"
                                value={data.date}
                                onChange={(e) => setData('date', e.target.value)}
                            />
                            {errors.date && <p className="text-sm text-red-600">{errors.date}</p>}
                        </div>

                        {/* Amounts */}
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label htmlFor="netto">Netto</Label>
                                <Input
                                    id="netto"
                                    type="number"
                                    step="0.01"
                                    value={data.netto}
                                    onChange={(e) => {
                                        setLastChanged("netto");
                                        setData('netto', parseFloat(e.target.value));
                                    }}
                                />
                            </div>
                            <div>
                                <Label htmlFor="mwstPercent">MwSt (%)</Label>
                                <Input
                                    id="mwstPercent"
                                    type="number"
                                    step="0.01"
                                    value={data.mwstPercent}
                                    onChange={(e) => {
                                        setLastChanged("mwstPercent");
                                        setData('mwstPercent', parseFloat(e.target.value));
                                    }}
                                />
                            </div>
                            <div>
                                <Label htmlFor="mwstEuro">MwSt (€)</Label>
                                <Input
                                    id="mwstEuro"
                                    type="number"
                                    step="0.01"
                                    value={data.mwstEuro}
                                    readOnly
                                    className="bg-muted cursor-not-allowed"
                                />
                            </div>

                            <div>
                                <Label htmlFor="brutto">Brutto</Label>
                                <Input
                                    id="brutto"
                                    type="number"
                                    step="0.01"
                                    value={data.brutto}
                                    onChange={(e) => {
                                        setLastChanged("brutto");
                                        setData('brutto', parseFloat(e.target.value));
                                    }}
                                />
                            </div>
                        </div>

                        {/* Parties */}
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label htmlFor="sender">Sender</Label>
                                <Input
                                    id="sender"
                                    value={data.sender}
                                    onChange={(e) => setData('sender', e.target.value)}
                                />
                            </div>
                            <div>
                                <Label htmlFor="receiver">Receiver</Label>
                                <Input
                                    id="receiver"
                                    value={data.receiver}
                                    onChange={(e) => setData('receiver', e.target.value)}
                                />
                            </div>
                        </div>

                        {/* Product */}
                        <div>
                            <Label htmlFor="product">Product</Label>
                            <Input
                                id="product"
                                value={data.product}
                                onChange={(e) => setData('product', e.target.value)}
                            />
                        </div>

                        {/* Currency + Method */}
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <Label htmlFor="currency">Currency</Label>
                                <Select
                                    value={data.currency}
                                    onValueChange={(value) => setData('currency', value)}
                                >
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select currency"/>
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value="EUR">EUR</SelectItem>
                                        <SelectItem value="USD">USD</SelectItem>
                                        <SelectItem value="CHF">CHF</SelectItem>
                                    </SelectContent>
                                </Select>
                            </div>

                            <div>
                                <Label htmlFor="method">Method</Label>
                                <Select
                                    value={data.method}
                                    onValueChange={(value) => setData('method', value)}
                                >
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select method"/>
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value="Überweisung">Überweisung</SelectItem>
                                        <SelectItem value="Bar">Bar</SelectItem>
                                        <SelectItem value="PayPal">PayPal</SelectItem>
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>

                        {/* Notes */}
                        <div>
                            <Label htmlFor="notes">Notes</Label>
                            <Textarea
                                id="notes"
                                value={data.notes}
                                onChange={(e) => setData('notes', e.target.value)}
                            />
                        </div>

                        {/* Optional Rechnung ID */}
                        <div>
                            <Label htmlFor="rechnungId">Invoice (optional)</Label>
                            <Popover open={searchOpen} onOpenChange={setSearchOpen}>
                                <PopoverTrigger asChild>
                                    <Button
                                        variant="outline"
                                        role="combobox"
                                        className="w-full justify-between"
                                    >
                                        {invoiceOptions.find(opt => opt.id === data.rechnungId)?.label ?? "Select Invoice"}
                                    </Button>
                                </PopoverTrigger>
                                <PopoverContent className="w-[300px] p-0">
                                    <Command shouldFilter={false}>
                                        <CommandInput
                                            placeholder="Search invoice..."
                                            onValueChange={handleSearch}
                                        />
                                        <CommandList>
                                            <CommandEmpty>No invoices found.</CommandEmpty>
                                            {invoiceOptions.map((invoice) => (
                                                <CommandItem
                                                    key={invoice.id}
                                                    value={invoice.label}
                                                    onSelect={() => {
                                                        setData("rechnungId", invoice.id);
                                                        setSearchOpen(false);
                                                    }}
                                                >
                                                    <Check
                                                        className={cn(
                                                            "mr-2 h-4 w-4",
                                                            data.rechnungId === invoice.id ? "opacity-100" : "opacity-0"
                                                        )}
                                                    />
                                                    {invoice.label}
                                                </CommandItem>
                                            ))}
                                        </CommandList>
                                    </Command>
                                </PopoverContent>
                            </Popover>
                        </div>
                    </CardContent>

                    <CardFooter className="justify-end">
                        <Button type="submit" disabled={processing}>
                            Create Payment
                        </Button>
                    </CardFooter>
                </Card>
            </form>

            <Dialog open={showSuccessModal} onOpenChange={setShowSuccessModal}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Success</DialogTitle>
                        <p className="text-muted-foreground">The payment was successfully saved.</p>
                    </DialogHeader>
                    <DialogFooter>
                        <Button onClick={() => router.visit('/Payments')}>OK</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </AppLayout>
    );
}
