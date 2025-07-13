import AppLayoutTemplate from '@/layouts/app/app-sidebar-layout';
import {type BreadcrumbItem} from '@/types';
import {type ReactNode} from 'react';
import {Head} from "@inertiajs/react";

interface AppLayoutProps {
    heading: string;
    headerChildren?: ReactNode[];
    leadingButtons?: ReactNode[];
    children: ReactNode;
    breadcrumbs?: BreadcrumbItem[];
}

export default (
    {
        heading,
        headerChildren,
        leadingButtons,
        children,
        breadcrumbs,
        ...props
    }: AppLayoutProps) => (
    <AppLayoutTemplate breadcrumbs={breadcrumbs} {...props}>
        <Head title="Dashboard"/>

        <div className="flex flex-1 flex-col gap-8 p-8 bg-background min-h-screen">
            <header className="space-y-2">
                <h1 className="text-4xl font-extrabold tracking-tight text-primary">
                    {heading}
                </h1>
                {headerChildren}
            </header>

            {leadingButtons && (
                <div className="flex flex-wrap gap-4">
                    {leadingButtons}
                </div>
            )}

            <section className="space-y-6">
                {children}
            </section>
        </div>
    </AppLayoutTemplate>
);
