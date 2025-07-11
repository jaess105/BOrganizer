import { Table, TableBody, TableCaption, TableHead, TableHeader, TableRow } from '@/components/ui/table';

export interface IMailOverviewElement {
    name: string;
    description: string;
    created_by: string;
    creation_date: Date;
    last_edited_by: string;
    last_edit_date: Date;
    times_used: number;
}

export type MailOverviewProps = {
    mailTemplates?: IMailOverviewElement[];
};

const demo: IMailOverviewElement = {
    name: 'Ask Library for abonnement',
    description: 'Template for asking a library to buy a certain abonnement',
    created_by: 'Jannik Esser',
    creation_date: new Date('2025-06-24T21:22:00Z'),
    last_edited_by: 'Jannik Esser',
    last_edit_date: new Date(),
    times_used: 5,
};

const MailOverview = ({ mailTemplates = [demo, demo, demo, demo, demo, demo, demo] }: MailOverviewProps) => {
    return (
        <Table>
            <TableCaption>Recently Created Mail Templates</TableCaption>
            <TableHeader>
                <TableRow>
                    <TableHead className="w-[100px]">Name</TableHead>
                    <TableHead>Description</TableHead>
                    <TableHead>Created by</TableHead>
                    <TableHead>Creation date</TableHead>
                    <TableHead>Last edited by</TableHead>
                    <TableHead>Last edit date</TableHead>
                    <TableHead className="text-right">Used</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {mailTemplates?.map((item, index) => (
                    <TableRow key={`template-overview-table-row-${index}`}>
                        <TableHead>{item.name}</TableHead>
                        <TableHead>{item.description}</TableHead>
                        <TableHead>{item.created_by}</TableHead>
                        <TableHead>{item.creation_date.toLocaleDateString()}</TableHead>
                        <TableHead>{item.last_edited_by}</TableHead>
                        <TableHead>{item.last_edit_date.toLocaleDateString()}</TableHead>
                        <TableHead className="text-right">{item.times_used}</TableHead>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );
};

export default MailOverview;
