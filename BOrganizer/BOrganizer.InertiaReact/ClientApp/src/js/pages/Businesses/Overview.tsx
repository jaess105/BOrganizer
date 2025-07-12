import {Head, Link} from '@inertiajs/react';
import {type BreadcrumbItem, PageProps} from '@/types';
import {Card, CardContent, CardFooter, CardHeader, CardTitle} from '@/components/ui/card';
import {Button} from '@/components/ui/button';
import AppLayout from "@/layouts/app-layout";
import {Business} from "@/types/busines";

interface Props extends PageProps {
    businesses: Business[];
}

const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Businesses',
        href: '/Businesses',
    },
];

export default function Overview({businesses}: Props) {
    return (
        <AppLayout
            breadcrumbs={breadcrumbs}
            heading={"All Businesses"}
            leadingButtons={[
                <Button asChild variant="secondary" size="lg">
                    <Link href="/Businesses/Create">Create a Business</Link>
                </Button>
            ]}

        >
            <Head title="Businesses"/>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {businesses.map((business) => (
                    <Card key={business.id} className="flex flex-col h-full justify-between">
                        <CardHeader>
                            <CardTitle>{business.name}</CardTitle>
                        </CardHeader>

                        <CardContent className="text space-y-2">
                            <div>
                                {business.street} {business.number}
                                <br/>
                                {business.plz} {business.ort}
                                {business.country && (
                                    <>
                                        <br/>
                                        {business.country}
                                    </>
                                )}
                            </div>

                            {business.tel && (
                                <div>
                                    <strong>Tel:</strong> {business.tel}
                                </div>
                            )}

                            {business.eMail && (
                                <div>
                                    <strong>Email:</strong>{' '}
                                    <a className="hover:underline" href={`mailto:${business.eMail}`}>
                                        {business.eMail}
                                    </a>
                                </div>
                            )}

                            {business.steuernummer && (
                                <div>
                                    <strong>Steuernummer:</strong> {business.steuernummer}
                                </div>
                            )}
                        </CardContent>

                        <CardFooter className="pt-4">
                            <Button variant="link" asChild>
                                <Link href={`/Businesses/Create?BusinessId=${business.id}`}>Edit</Link>
                            </Button>
                        </CardFooter>
                    </Card>
                ))}
            </div>
        </AppLayout>
    );
}
