import {useForm} from '@inertiajs/react';
import React, {useState} from 'react';
import AppLayout from '@/layouts/app-layout';
import RechnungPdfFrame from "@/pages/Rechnungen/RechnungPdfFrame";
import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectLabel,
    SelectTrigger,
    SelectValue
} from "@/components/ui/select";
import {Label} from "@/components/ui/label";
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"
import {Button} from "@/components/ui/button";
import {ChevronDownIcon} from "lucide-react";
import {Calendar} from "@/components/ui/calendar"
import type {BreadcrumbItem} from "@/types";


type Business = {
    id: number;
    name: string;
};

type Credit = {
    id: number;
    short: string;
};

type Steuersatz = {
    id: number;
    steuerSatz: string;
    inProzent: number;
};

type InvoiceItem = {
    beschreibung: string;
    quantity: number;
    unitPrice: number;
    priceMwst: number;
};

type InvoiceDto = {
    invoiceId: number;
    rechnungsNummer: string;
    rechnungsStellerId: number;
    rechnungsEmpfaengerId: number;
    rechnungsDatum: Date;
    lieferDatum: Date;
    steuersatzId: number;
    creditId: number;
    invoiceItems: InvoiceItem[];
};


type Props = {
    businesses: Business[];
    credits: Credit[];
    steuersaetze: Steuersatz[];
    invoiceDto: InvoiceDto | null;
};

const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Rechnungen',
        href: '/Rechnungen',
    },
    {
        title: 'Create Rechnung',
        href: '/Rechnungen/Create',
    },
];

function toDate(date?: Date | undefined): Date {
    if (date == undefined) {
        return new Date();
    }
    return new Date(date);
}

export default function RechnungCreationForm({businesses, credits, steuersaetze, invoiceDto}: Props) {
    const [selectedMwst, setSelectedMwst] = useState<number>(steuersaetze[0]?.inProzent ?? 0);
    const {data, setData, post, processing, errors} = useForm({
        invoiceId: invoiceDto?.invoiceId ?? null,
        rechnungsStellerId: invoiceDto?.rechnungsStellerId ?? '',
        rechnungsEmpfaengerId: invoiceDto?.rechnungsEmpfaengerId ?? '',
        rechnungsDatum: toDate(invoiceDto?.rechnungsDatum),
        lieferDatum: toDate(invoiceDto?.lieferDatum),
        steuersatzId: invoiceDto?.steuersatzId ?? (steuersaetze[0]?.id) ?? '',
        creditId: invoiceDto?.creditId ?? '',
        invoiceItems: invoiceDto?.invoiceItems ?? ([] as InvoiceItem[]),
    });

    const updateItem = (index: number, key: keyof InvoiceItem, value: string | number) => {
        const items = [...data.invoiceItems];
        const item = {...items[index]};

        if (key === 'unitPrice') {
            item.unitPrice = Number(value);
            item.priceMwst = +(item.unitPrice * (1 + selectedMwst)).toFixed(2);
        } else if (key === 'priceMwst') {
            item.priceMwst = Number(value);
            item.unitPrice = +(item.priceMwst / (1 + selectedMwst)).toFixed(2);
        } else if (key === 'quantity') {
            item.quantity = Number(value);
        } else {
            item.beschreibung = value.toString();
        }

        items[index] = item;
        setData('invoiceItems', items);
    };

    const addItem = () => {
        setData('invoiceItems', [
            ...data.invoiceItems,
            {beschreibung: '', quantity: 1, unitPrice: 0, priceMwst: 0},
        ]);
    };

    const removeItem = (index: number) => {
        setData(
            'invoiceItems',
            data.invoiceItems.filter((_, i) => i !== index)
        );
    };

    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        post('/Rechnungen/Create', {
            forceFormData: true,
            onSuccess: () => setShowSuccessModal(true),
            onError: () => console.error('Submission failed')
        });
    };

    // date picker
    const [rechnungsDatumOpen, setRechnungsDatumOpen] = React.useState(false)
    const [lieferDatumOpen, setLieferDatumOpen] = React.useState(false)

    return (
        <AppLayout breadcrumbs={breadcrumbs}
                   heading={"Rechnung Creation"}>
            <div className="space-y-8 p-6">
                <form onSubmit={handleSubmit}>
                    {/* Rechnungssteller & Empfänger */}
                    <div className="grid grid-cols-2 gap-6">
                        <div>
                            <label htmlFor="rechnungsStellerId" className="block text font-medium  mb-1">
                                Rechnungssteller
                            </label>
                            <Select
                                value={data.rechnungsStellerId.toString()}
                                onValueChange={(e) => setData('rechnungsStellerId', e)}
                            >
                                <SelectTrigger
                                    className="w-full rounded-md border py-2 px-3 "
                                >
                                    <SelectValue placeholder="Bitte wählen"/>
                                </SelectTrigger>
                                <SelectContent>
                                    {businesses.map((b) => (
                                        <SelectItem key={b.id} value={b.id.toString()}>
                                            {b.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            {errors.rechnungsStellerId && (
                                <p className="mt-1 text-sm text-red-600">{errors.rechnungsStellerId}</p>
                            )}
                        </div>

                        <div>
                            <Label htmlFor="rechnungsEmpfaengerId" className="block text-sm font-medium  mb-1">
                                Rechnungsempfänger
                            </Label>
                            <Select
                                value={data.rechnungsEmpfaengerId.toString()}
                                onValueChange={(e) => setData('rechnungsEmpfaengerId', e)}
                            >
                                <SelectTrigger
                                    className="w-full rounded-md border py-2 px-3 "
                                >
                                    <SelectValue placeholder="Bitte wählen"/>
                                </SelectTrigger>
                                <SelectContent>
                                    {businesses.map((b) => (
                                        <SelectItem key={b.id} value={b.id.toString()}>
                                            {b.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            {errors.rechnungsEmpfaengerId && (
                                <p className="mt-1 text-sm text-red-600">{errors.rechnungsEmpfaengerId}</p>
                            )}
                        </div>
                    </div>

                    {/* Rechnungsdatum & Lieferdatum */}
                    <div className="grid grid-cols-2 gap-6">
                        <div className="flex flex-col gap-3">
                            <Label htmlFor="rechnungsdatum" className="px-1">
                                Rechnungsdatum
                            </Label>
                            <Popover open={rechnungsDatumOpen} onOpenChange={setRechnungsDatumOpen}>
                                <PopoverTrigger asChild>
                                    <Button
                                        variant="outline"
                                        id="rechnungsdatum"
                                        className="w-full justify-between font-normal"
                                    >
                                        {data.rechnungsDatum ? data.rechnungsDatum.toLocaleDateString() : "Select date"}
                                        <ChevronDownIcon/>
                                    </Button>
                                </PopoverTrigger>
                                <PopoverContent className="w-auto overflow-hidden p-0" align="start">
                                    <Calendar
                                        mode="single"
                                        selected={data.rechnungsDatum}
                                        captionLayout="dropdown"
                                        onSelect={(date) => {
                                            if (date != undefined) {
                                                setData('rechnungsDatum', date);
                                            }
                                            setRechnungsDatumOpen(false);
                                        }}
                                    />
                                </PopoverContent>
                            </Popover>
                            {errors.rechnungsDatum && (
                                <p className="mt-1 text-sm text-red-600">{errors.rechnungsDatum}</p>
                            )}
                        </div>

                        <div className="flex flex-col gap-3">
                            <Label htmlFor="lieferDatum" className="px-1">
                                Lieferdatum
                            </Label>
                            <Popover open={lieferDatumOpen} onOpenChange={setLieferDatumOpen}>
                                <PopoverTrigger asChild>
                                    <Button
                                        variant="outline"
                                        id="lieferDatum"
                                        className="w-full justify-between font-normal"
                                    >
                                        {data.lieferDatum ? data.lieferDatum.toLocaleDateString() : "Select date"}
                                        <ChevronDownIcon/>
                                    </Button>
                                </PopoverTrigger>
                                <PopoverContent className="w-auto overflow-hidden p-0" align="start">
                                    <Calendar
                                        mode="single"
                                        selected={data.lieferDatum}
                                        captionLayout="dropdown"
                                        onSelect={(date) => {
                                            if (date != undefined) {
                                                setData('lieferDatum', date);
                                            }
                                            setLieferDatumOpen(false);
                                        }}
                                    />
                                </PopoverContent>
                            </Popover>
                            {errors.rechnungsDatum && (
                                <p className="mt-1 text-sm text-red-600">{errors.rechnungsDatum}</p>
                            )}
                        </div>
                    </div>

                    {/* Steuersatz */}
                    <div>
                        <label htmlFor="steuersatzId" className="block text-sm font-medium  mb-1">
                            Steuersatz
                        </label>
                        <select
                            id="steuersatzId"
                            value={data.steuersatzId}
                            onChange={(e) => {
                                const id = Number(e.target.value);
                                const mwst = steuersaetze.find((s) => s.id === id)?.inProzent ?? 0;
                                setSelectedMwst(mwst);
                                setData('steuersatzId', id);
                            }}
                            className="block w-full rounded-md border py-2 px-3 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                        >
                            {steuersaetze.map((s) => (
                                <option key={s.id} value={s.id}>
                                    {s.steuerSatz} ({s.inProzent}%)
                                </option>
                            ))}
                        </select>
                        {errors.steuersatzId && (
                            <p className="mt-1 text-sm text-red-600">{errors.steuersatzId}</p>
                        )}
                    </div>

                    {/* Zahlungskonto */}
                    <div>
                        <label htmlFor="creditId" className="block text-sm font-medium  mb-1">
                            Zahlungskonto
                        </label>
                        <select
                            id="creditId"
                            value={data.creditId}
                            onChange={(e) => setData('creditId', e.target.value)}
                            className="block w-full rounded-md border   py-2 px-3 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                        >
                            <option value="">Bitte wählen</option>
                            {credits.map((c) => (
                                <option key={c.id} value={c.id}>
                                    {c.short}
                                </option>
                            ))}
                        </select>
                        {errors.creditId && (
                            <p className="mt-1 text-sm text-red-600">{errors.creditId}</p>
                        )}
                    </div>

                    {/* Invoice Items */}
                    <div>
                        <h4 className="text-lg font-semibold mb-4">Rechnungspositionen</h4>

                        <table className="w-full table-auto border  rounded-md overflow-hidden">
                            <thead>
                            <tr>
                                <th className="border-b  px-4 py-2 text-left text-sm font-medium ">
                                    Bezeichnung
                                </th>
                                <th className="border-b  px-4 py-2 text-left text-sm font-medium ">
                                    Anzahl
                                </th>
                                <th className="border-b  px-4 py-2 text-left text-sm font-medium ">
                                    Einzelpreis
                                </th>
                                <th className="border-b  px-4 py-2 text-left text-sm font-medium ">
                                    Inkl. MwSt
                                </th>
                                <th className="border-b  px-4 py-2"></th>
                            </tr>
                            </thead>

                            <tbody>
                            {data.invoiceItems.map((item, i) => (
                                <tr key={i}>
                                    <td className="border-b  px-4 py-2">
                                        <input
                                            type="text"
                                            value={item.beschreibung}
                                            onChange={(e) => updateItem(i, 'beschreibung', e.target.value)}
                                            className="block w-full rounded-md border  py-1 px-2 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                                            placeholder="Bezeichnung"
                                        />
                                    </td>
                                    <td className="border-b  px-4 py-2">
                                        <input
                                            type="number"
                                            min={0}
                                            value={item.quantity}
                                            onChange={(e) => updateItem(i, 'quantity', e.target.value)}
                                            className="block w-full rounded-md border  py-1 px-2 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                                            placeholder="Anzahl"
                                        />
                                    </td>
                                    <td className="border-b  px-4 py-2">
                                        <input
                                            type="number"
                                            min={0}
                                            step={0.01}
                                            value={item.unitPrice}
                                            onChange={(e) => updateItem(i, 'unitPrice', e.target.value)}
                                            className="block w-full rounded-md border  py-1 px-2 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                                            placeholder="Einzelpreis"
                                        />
                                    </td>
                                    <td className="border-b  px-4 py-2">
                                        <input
                                            type="number"
                                            min={0}
                                            step={0.01}
                                            value={item.priceMwst}
                                            onChange={(e) => updateItem(i, 'priceMwst', e.target.value)}
                                            className="block w-full rounded-md border  py-1 px-2 shadow-sm focus:border-primary focus:ring focus:ring-primary/50 focus:ring-opacity-50"
                                            placeholder="Inkl. MwSt"
                                        />
                                    </td>
                                    <td className="border-b  px-4 py-2 text-center">
                                        <button
                                            type="button"
                                            onClick={() => removeItem(i)}
                                            className="text-red-600 hover:text-red-800 transition"
                                            aria-label="Remove item"
                                        >
                                            ×
                                        </button>
                                    </td>
                                </tr>
                            ))}
                            </tbody>
                        </table>

                        <button
                            type="button"
                            onClick={addItem}
                            className="mt-3 inline-flex items-center rounded-md bg-secondary px-4 py-2 text-sm font-medium  hover:bg-secondary-dark focus:outline-none focus:ring-2 focus:ring-secondary focus:ring-offset-2"
                        >
                            + Item
                        </button>
                    </div>

                    <button
                        type="submit"
                        disabled={processing}
                        className="inline-flex justify-center rounded-md  px-6 py-3 text-lg font-semibold focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 disabled:opacity-50"
                    >
                        Rechnung speichern
                    </button>
                </form>

                {invoiceDto?.invoiceId != null && (
                    <RechnungPdfFrame invoiceId={invoiceDto!.invoiceId!} alwaysShow={false}/>
                )}
            </div>
        </AppLayout>
    );
}
