import {
    Euro,
    Banknote,
    HandCoins,
    FileText,
} from "lucide-react";
import {Card, CardHeader, CardTitle, CardContent} from "@/components/ui/card";

export function EarningsSummaryCard(
    {
        netto,
        brutto,
        mwst,
    }: {
        netto: number;
        brutto: number;
        mwst: number;
    }) {
    return (
        <Card className="max-w-3xl mx-auto mb-6">
            <CardHeader>
                <CardTitle className="text-xl flex items-center gap-2">
                    <FileText className="w-5 h-5 text-blue-500"/>
                    Einnahmen Übersicht
                </CardTitle>
            </CardHeader>

            <CardContent className="grid grid-cols-1 sm:grid-cols-3 gap-6 text-sm text-muted-foreground">
                <div className="space-y-1">
                    <div className="flex items-center gap-2 text-green-600 font-semibold">
                        <HandCoins className="w-4 h-4"/>
                        Einnahmen Netto
                    </div>
                    <div className="text-base text-foreground font-medium">
                        {netto.toFixed(2)} €
                    </div>
                    <p className="text-xs">Real Gewinn</p>
                </div>

                <div className="space-y-1">
                    <div className="flex items-center gap-2 text-blue-600 font-semibold">
                        <Banknote className="w-4 h-4"/>
                        Einnahmen Brutto
                    </div>
                    <div className="text-base text-foreground font-medium">
                        {brutto.toFixed(2)} €
                    </div>
                    <p className="text-xs">Einnahmen vor Abzug der MwSt</p>
                </div>

                <div className="space-y-1">
                    <div className="flex items-center gap-2 text-yellow-600 font-semibold">
                        <Euro className="w-4 h-4"/>
                        Einnahmen MwSt
                    </div>
                    <div className="text-base text-foreground font-medium">
                        {mwst.toFixed(2)} €
                    </div>
                    <p className="text-xs">Muss an den Bund abgeführt werden</p>
                </div>
            </CardContent>
        </Card>
    );
}
