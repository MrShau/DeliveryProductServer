//export const API_BASE: string = "https://localhost:7002";
export const API_BASE: string = "https://deliveryproductapi.bsite.net";

export const LOGIN_MIN = 3;
export const LOGIN_MAX = 64;

export const PASSWORD_MIN = 7;
export const PASSWORD_MAX = 255;

export function getToken(): string | null {
    if (localStorage.getItem("token") == null)
        return null;
    return `Bearer ${localStorage.getItem('token')}`
}

export function getDateTime(date: Date) : string {
    if (date != null) {
        date = new Date(date);
        return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    }
    
    return "";
}