import AppLayout from '@/layouts/app-layout';
import { BreadcrumbItem } from '@/types';
import { Head, useForm } from '@inertiajs/react';
import { useState } from 'react'; // 👈 CHANGES
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Label } from '@/components/ui/label';
import { CardHeader, Card, CardTitle, CardContent } from "@/components/ui/card";
import { SelectContent, SelectItem, SelectGroup, SelectTrigger, SelectValue, Select } from '@/components/ui/select';


const breadcrumbs: BreadcrumbItem[] = [
    {
        title: 'Template Creation',
        href: '/templates/create',
    },
];

type Variable = {
    name: string;
    type: 'InputVar' | 'LinkVar';
};

export default function Create() {
    const { data, setData, post, processing, errors } = useForm({
        name: '',
        description: '',
        text: '',
        variables: [{ name: '', type: 'InputVar' }],
    });

    const [previewVariables, setPreviewVariables] = useState<Record<string, string>>({}); // 👈 CHANGES

    const handleVariableChange = (index: number, field: keyof Variable, value: string) => {
        const updated = [...data.variables];
        updated[index] = {
            ...updated[index],
            [field]: value,
        };
        setData('variables', updated);
    };

    const addVariable = () => {
        setData('variables', [...data.variables, { name: '', type: 'InputVar' }]);
    };

    const handlePreviewInputChange = (name: string, value: string) => {
        setPreviewVariables(prev => ({
            ...prev,
            [name]: value,
        }));
    };

    const submit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        post(route('templates.store'));
    };

    const renderPreviewText = () => {
        let output = data.text;
        data.variables.forEach(v => {
            const pattern = new RegExp(`\\$\\{${v.name}\\}`, 'g');
            const value = previewVariables[v.name] ?? `[${v.name}]`;
            output = output.replace(pattern, value);
        });
        return output;
    };

    return (
        <AppLayout breadcrumbs={breadcrumbs}>
            <Head title="Create Template" />
            <div className="flex h-full flex-1 flex-col gap-4 rounded-xl p-4 overflow-x-auto">
                <div className="relative min-h-[100vh] flex-1 overflow-hidden rounded-xl border border-sidebar-border/70 md:min-h-min dark:border-sidebar-border">
                    <form onSubmit={submit} className="space-y-6 p-4">
                        <div className="space-y-2">
                            <Label htmlFor="name">Template Name</Label>
                            <Input
                                id="name"
                                value={data.name}
                                onChange={(e) => setData('name', e.target.value)}
                            />
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="description">Template Description</Label>
                            <Input
                                id="description"
                                value={data.description}
                                onChange={(e) => setData('description', e.target.value)}
                            />
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="text">Text</Label>
                                <Textarea
                                    id="text"
                                    rows={10}
                                    value={data.text}
                                    onChange={(e) => setData('text', e.target.value)}
                                />
                            </div>

                            <div className="space-y-2">
                                <Label>Variables</Label>
                                {data.variables.map((v, i) => (
                                    <div key={i} className="flex items-center gap-2">
                                        <Input
                                            placeholder="Name"
                                            value={v.name}
                                            onChange={(e) => handleVariableChange(i, 'name', e.target.value)}
                                        />
                                        <Select
                                            value={v.type}
                                            onValueChange={(value: any) =>
                                                handleVariableChange(i, 'type', value as 'InputVar' | 'LinkVar')
                                            }
                                        >
                                            <SelectTrigger className="w-[120px]">
                                                <SelectValue />
                                            </SelectTrigger>
                                            <SelectContent>
                                                <SelectItem value="InputVar">Input Var</SelectItem>
                                                <SelectItem value="LinkVar">Link Var</SelectItem>
                                            </SelectContent>
                                        </Select>
                                    </div>
                                ))}
                                <Button type="button" variant="outline" onClick={addVariable}>
                                    + Add Variable
                                </Button>
                            </div>
                        </div>

                        <Button type="submit" disabled={processing}>
                            Save Template
                        </Button>
                    </form>


                    {/* 👇 PREVIEW SECTION */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4 border-t mt-8">
                        <Card>
                            <CardHeader>
                                <CardTitle>Preview Text</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="whitespace-pre-wrap">{renderPreviewText()}</div>
                            </CardContent>
                        </Card>

                        <Card>
                            <CardHeader>
                                <CardTitle>Preview Variables</CardTitle>
                            </CardHeader>
                            <CardContent className="space-y-2">
                                {data.variables.map((v, i) => (
                                    <div key={i} className="flex items-center gap-2">
                                        <Label className="w-24 truncate">{v.name || '(unnamed)'}</Label>
                                        <Input
                                            placeholder="Value"
                                            value={previewVariables[v.name] || ''}
                                            onChange={(e) => handlePreviewInputChange(v.name, e.target.value)}
                                        />
                                    </div>
                                ))}
                            </CardContent>
                        </Card>
                    </div>

                </div>
            </div>
        </AppLayout>
    );
}
