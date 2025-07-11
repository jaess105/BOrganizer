import React, {useState} from 'react';
import {Button} from "@/components/ui/button";

type PdfViewerProps = {
    invoiceId: number;
    alwaysShow?: boolean;
};

export default function RechnungPdfFrame({invoiceId, alwaysShow = false}: PdfViewerProps) {
    const [displayPdf, setDisplayPdf] = useState(alwaysShow);
    const [reloadKey, setReloadKey] = useState(0);

    const pdfUrl = `/Api/Rechnung/Creation/Pdf/${invoiceId}`;

    const reloadPdf = () => {
        setReloadKey((prev) => prev + 1);
    };

    const toggleDisplayPdf = () => {
        setDisplayPdf((prev) => !prev);
    };

    return (
        <div>
            <div className="flex items-center mb-4 space-x-4">
                {!alwaysShow && (
                    <Button
                        onClick={toggleDisplayPdf}
                        className="rounded bg-gray-600 px-4 py-2 text-white hover:bg-gray-700 w-28 text-center"
                    >
                        {displayPdf ? 'Hide PDF' : 'Show PDF'}
                    </Button>
                )}


                {displayPdf && (
                    <Button
                        onClick={reloadPdf}
                        className="rounded px-4 py-2"
                    >
                        PDF neu laden
                    </Button>
                )}
            </div>

            {displayPdf && (
                <iframe
                    key={reloadKey}
                    src={pdfUrl}
                    width="100%"
                    height="800px"
                    style={{border: 'none'}}
                    title="Rechnung PDF"
                />
            )}
        </div>
    );
}
