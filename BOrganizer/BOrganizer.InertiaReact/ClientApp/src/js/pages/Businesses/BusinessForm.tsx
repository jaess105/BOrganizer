import {useState} from 'react';
import {router, useForm} from '@inertiajs/react';
import {Input} from '@/components/ui/input';
import {Label} from '@/components/ui/label';
import {Button} from '@/components/ui/button';
import AppLayout from '@/layouts/app-layout';
import {Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle} from '@/components/ui/dialog';

type Props = {
    business: {
        id?: number;
        name: string;
        street: string;
        number: string;
        plz: string;
        ort: string;
        tel?: string;
        eMail?: string;
        country?: string;
        steuernummer?: string;
    } | null;
    firstName: string;
    lastName: string;
    isUsersBusiness: boolean;
};

type FormData = {
    id?: number;
    name: string;
    street: string;
    number: string;
    plz: string;
    ort: string;
    tel?: string;
    eMail?: string;
    country?: string;
    steuernummer?: string;
    firstName: string;
    lastName: string;
    isUsersBusiness: boolean;
};

export default function BusinessFormPage({
                                             business,
                                             firstName,
                                             lastName,
                                             isUsersBusiness
                                         }: Props) {
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const {data, setData, post, processing, errors, transform} = useForm<FormData>({
        id: business?.id,
        name: business?.name ?? '',
        street: business?.street ?? '',
        number: business?.number ?? '',
        plz: business?.plz ?? '',
        ort: business?.ort ?? '',
        tel: business?.tel ?? '',
        eMail: business?.eMail ?? '',
        country: business?.country ?? '',
        steuernummer: business?.steuernummer ?? '',
        firstName: firstName ?? '',
        lastName: lastName ?? '',
        isUsersBusiness: isUsersBusiness ?? false
    });


    // transform((data ) => {
    //     return ;
    // })

    function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        post('/Businesses/Create', {
            forceFormData: true,
            onSuccess: () => setShowSuccessModal(true),
            onError: () => console.error('Submission failed')
        });
    }

    return (
        <AppLayout>
            <form onSubmit={handleSubmit} className="space-y-6 max-w-3xl mx-auto p-6">
                <h1 className="text-2xl font-bold">
                    {data.id ? 'Edit Business' : 'Create Business'}
                </h1>

                {/* Business Info */}
                <fieldset className="space-y-4">
                    <div>
                        <Label htmlFor="name">Name</Label>
                        <Input
                            id="name"
                            value={data.name}
                            onChange={(e) => setData('name', e.target.value)}
                        />
                        {errors.name && <p className="text-red-500 text-sm">{errors.name}</p>}
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <Label htmlFor="steuernummer">Steuernummer</Label>
                            <Input
                                id="steuernummer"
                                value={data.steuernummer}
                                onChange={(e) => setData('steuernummer', e.target.value)}
                            />
                        </div>
                        <div>
                            <Label htmlFor="street">Street</Label>
                            <Input
                                id="street"
                                value={data.street}
                                onChange={(e) => setData('street', e.target.value)}
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-3 gap-4">
                        <div>
                            <Label htmlFor="number">Number</Label>
                            <Input
                                id="number"
                                value={data.number}
                                onChange={(e) => setData('number', e.target.value)}
                            />
                        </div>
                        <div>
                            <Label htmlFor="plz">PLZ</Label>
                            <Input
                                id="plz"
                                value={data.plz}
                                onChange={(e) => setData('plz', e.target.value)}
                            />
                        </div>
                        <div>
                            <Label htmlFor="ort">Ort</Label>
                            <Input
                                id="ort"
                                value={data.ort}
                                onChange={(e) => setData('ort', e.target.value)}
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <Label htmlFor="tel">Tel</Label>
                            <Input
                                id="tel"
                                value={data.tel}
                                onChange={(e) => setData('tel', e.target.value)}
                            />
                        </div>
                        <div>
                            <Label htmlFor="eMail">Email</Label>
                            <Input
                                id="eMail"
                                type="email"
                                value={data.eMail}
                                onChange={(e) => setData('eMail', e.target.value)}
                            />
                        </div>
                    </div>

                    <div>
                        <Label htmlFor="country">Country</Label>
                        <Input
                            id="country"
                            value={data.country}
                            onChange={(e) => setData('country', e.target.value)}
                        />
                    </div>
                </fieldset>

                {/* Contact Person */}
                <fieldset className="space-y-4">
                    <h2 className="text-lg font-semibold">Contact Person (optional)</h2>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <Label htmlFor="firstName">First Name</Label>
                            <Input
                                id="firstName"
                                value={data.firstName}
                                onChange={(e) => setData('firstName', e.target.value)}
                            />
                        </div>
                        <div>
                            <Label htmlFor="lastName">Last Name</Label>
                            <Input
                                id="lastName"
                                value={data.lastName}
                                onChange={(e) => setData('lastName', e.target.value)}
                            />
                        </div>
                    </div>
                </fieldset>

                {/* Is Users Business */}
                <div className="flex items-center gap-2">
                    <input
                        id="isUsersBusiness"
                        type="checkbox"
                        checked={data.isUsersBusiness}
                        onChange={(e) => setData('isUsersBusiness', e.target.checked)}
                    />
                    <Label htmlFor="isUsersBusiness">Is this your business?</Label>
                </div>

                <Button type="submit" disabled={processing}>
                    {data.id ? 'Save Changes' : 'Create'}
                </Button>
            </form>

            {/* Success Modal */}
            <Dialog open={showSuccessModal} onOpenChange={setShowSuccessModal}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Success</DialogTitle>
                        <p>The business has been successfully saved.</p>
                    </DialogHeader>
                    <DialogFooter>
                        <Button onClick={() => router.visit('/')}>OK</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </AppLayout>
    );
}
