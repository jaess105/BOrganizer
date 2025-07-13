import {ColumnDef} from "@tanstack/react-table";
import {PaymentDto} from "@/types/payment";
import {Badge} from "@/components/ui/badge";
import {Button} from "@/components/ui/button";
import {Link, router} from "@inertiajs/react";
import {
    Dialog,
    DialogTrigger,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from "@/components/ui/dialog";
import {useState} from "react";

export const columns: ColumnDef<PaymentDto>[] = [
    {
        accessorKey: "product",
        header: "Product",
    },
    {
        accessorKey: "date",
        header: "Date",
        cell: ({row}) => {
            const date = new Date(row.getValue("date"));
            return date.toLocaleDateString();
        },
    },
    {
        accessorKey: "sender",
        header: "Sender",
    },
    {
        accessorKey: "receiver",
        header: "Receiver",
    },
    {
        accessorKey: "brutto",
        header: "Amount",
        cell: ({row}) => {
            const brutto = row.getValue("brutto") as number;
            const currency = row.original.currency;
            return (
                <span>
                    {brutto.toFixed(2)}{" "}
                    <span className="text-muted-foreground">{currency}</span>
                </span>
            );
        },
    },
    {
        accessorKey: "method",
        header: "Method",
        cell: ({row}) => <Badge variant="outline">{row.getValue("method")}</Badge>,
    },
    {
        accessorKey: "rechnungId",
        header: "Invoice",
        cell: ({row}) => {
            const id = row.getValue("rechnungId");
            return id ? (
                <Link href={`/Invoices/${id}`} className="text-blue-600 underline">
                    #{id.toString()}
                </Link>
            ) : (
                <span className="text-muted-foreground">–</span>
            );
        },
    },
    {
        id: "actions",
        header: "",
        cell: ({row}) => {
            const id = row.original.paymentId;
            const [open, setOpen] = useState(false);

            const handleDelete = () => {
                router.delete(`/Payments/Delete/${id}`, {
                    onFinish: () => setOpen(false),
                });
            };

            return (
                <>
                    <div className="flex space-x-2">
                        <Button variant="outline" asChild>
                            <Link href={`/Payments/Create?paymentId=${id}`}>Edit</Link>
                        </Button>

                        <Dialog open={open} onOpenChange={setOpen}>
                            <DialogTrigger asChild>
                                <Button variant="destructive">Delete</Button>
                            </DialogTrigger>
                            <DialogContent>
                                <DialogHeader>
                                    <DialogTitle>Delete Payment</DialogTitle>
                                    <p>Are you sure you want to delete this payment?</p>
                                </DialogHeader>
                                <DialogFooter>
                                    <Button variant="outline" onClick={() => setOpen(false)}>
                                        Cancel
                                    </Button>
                                    <Button variant="destructive" onClick={handleDelete}>
                                        Confirm
                                    </Button>
                                </DialogFooter>
                            </DialogContent>
                        </Dialog>
                    </div>
                </>
            );
        },
    },
];
