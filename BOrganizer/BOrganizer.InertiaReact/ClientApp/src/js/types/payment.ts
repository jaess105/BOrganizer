export type Payment = {
    paymentId: number;
    date: string;
    sender: string;
    receiver: string;
    product: string;
    currency: string;
    method: string;
    brutto: number;
    notes?: string;
    rechnungId?: number;
};

export interface PaymentDto {
    paymentId?: number;
    date: string;
    netto: number;
    mwstPercent: number;
    mwstEuro: number;
    brutto: number;
    sender: string;
    receiver: string;
    product: string;
    currency: string;
    method: string;
    notes?: string;
    rechnungId?: number;
    senderId?: number;
    receiverId?: number;
}