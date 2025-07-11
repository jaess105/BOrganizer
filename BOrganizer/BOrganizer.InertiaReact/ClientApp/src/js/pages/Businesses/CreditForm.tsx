import {Head, router, useForm} from '@inertiajs/react';
import {PageProps} from '@/types';
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    CardFooter,
} from '@/components/ui/card';
import {Input} from '@/components/ui/input';
import {Label} from '@/components/ui/label';
import {Button} from '@/components/ui/button';
import AppLayout from "@/layouts/app-layout";
import {Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog";
import {useState} from "react";

export default function CreditForm({}: PageProps) {
    const {data, setData, post, errors, processing} = useForm({
        institute: '',
        iban: '',
        bic: '',
        inhaber: '',
    });

    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        post('/Businesses/Credit/Create', {
            forceFormData: true,
            onSuccess: () => setShowSuccessModal(true),
            onError: () => console.error('Submission failed')
        }); // Match your controller route
    };

    return (
        <AppLayout>
            <Head title="Kreditverbindung erstellen"/>

            <div className="max-w-xl p-6">
                <Card>
                    <CardHeader>
                        <CardTitle>Kreditverbindung erstellen</CardTitle>
                    </CardHeader>

                    <form onSubmit={handleSubmit}>
                        <CardContent className="space-y-4">
                            <div>
                                <Label htmlFor="institute">Institut</Label>
                                <Input
                                    id="institute"
                                    value={data.institute}
                                    onChange={(e) => setData('institute', e.target.value)}
                                />
                                {errors.institute && (
                                    <p className="text-sm text-red-600 mt-1">{errors.institute}</p>
                                )}
                            </div>

                            <div>
                                <Label htmlFor="iban">IBAN</Label>
                                <Input
                                    id="iban"
                                    value={data.iban}
                                    onChange={(e) => setData('iban', e.target.value)}
                                />
                                {errors.iban && (
                                    <p className="text-sm text-red-600 mt-1">{errors.iban}</p>
                                )}
                            </div>

                            <div>
                                <Label htmlFor="bic">BIC</Label>
                                <Input
                                    id="bic"
                                    value={data.bic}
                                    onChange={(e) => setData('bic', e.target.value)}
                                />
                                {errors.bic && (
                                    <p className="text-sm text-red-600 mt-1">{errors.bic}</p>
                                )}
                            </div>

                            <div>
                                <Label htmlFor="inhaber">Kontoinhaber</Label>
                                <Input
                                    id="inhaber"
                                    value={data.inhaber}
                                    onChange={(e) => setData('inhaber', e.target.value)}
                                />
                                {errors.inhaber && (
                                    <p className="text-sm text-red-600 mt-1">{errors.inhaber}</p>
                                )}
                            </div>
                        </CardContent>


                        <CardFooter className="pt-4">
                            <Button type="submit" disabled={processing}>
                                Speichern
                            </Button>
                        </CardFooter>
                    </form>
                </Card>
            </div>

            {/* Success Modal */}
            <Dialog open={showSuccessModal} onOpenChange={setShowSuccessModal}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Success</DialogTitle>
                        <p>The credit card information has been successfully created.</p>
                    </DialogHeader>
                    <DialogFooter>
                        <Button onClick={() => router.visit('/')}>OK</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </AppLayout>
    );
}
