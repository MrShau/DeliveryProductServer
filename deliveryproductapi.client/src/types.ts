export type UserType = {
    id: number,
    email?: string,
    login: string,
    role: string
}

export type CategoryType = {
    id: number,
    name: string
}

export type ProductType = {
    id: number,
    title: string,
    description: string,
    price: number,
    imagePath: string,
    categoryId: number | null,
    categoryName: string | null,
    weight: number,
    weightUnit: string,
    count: number
}

export type OrderProductItemType = {
    product: ProductType,
    count: number
}

export type OrderType = {
    id: number,
    products: OrderProductItemType[],
    deliveryPrice: number,
    deliveryTime: number,
    statusId: number,
    status: string,
    isCompleted: boolean,
    totalPrice: number,
    createdAt: Date,
    clientId: number,
    clientLogin: string,
    address: string
}

export type ChatType = {
    id: number,
    clientId: number,
    clientLogin: string,
    orderId: number,
    lastMessage: string,
    lastMessageDate: Date
}

export type MessageType = {
    id: number,
    content: string,
    senderId: number,
    senderLogin: string,
    chatId: number,
    createdAt: Date,
    attachmentPath: string
}