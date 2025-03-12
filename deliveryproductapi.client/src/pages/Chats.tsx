import { Button, Col, Container, Form, Modal, Row } from "react-bootstrap";
import Sidebar from "../components/Sidebar";
import { useContext, useEffect, useRef, useState } from "react";
import { ChatType, MessageType, OrderType } from "../types";
import ChatApi from "../api/ChatApi";

import * as signalR from "@microsoft/signalr";
import { API_BASE, getDateTime } from "../consts";
import { Context } from "../main";
import { MdAttachFile, MdSend } from "react-icons/md";
import { FaUser } from "react-icons/fa";
import { useParams } from "react-router-dom";
import OrderApi from "../api/OrderApi";


export default function Chats() {

    const params: any = useParams();

    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [orderShow, setOrderShow] = useState(false);
    const [order, setOrder] = useState<OrderType | null>(null);

    const [chats, setChats] = useState<ChatType[]>([]);
    const [messages, setMessages] = useState<MessageType[]>([]);
    const [chatId, setChatId] = useState<number>(0);

    const [message, setMessage] = useState<string>("");;

    const messagesDivRef = useRef<HTMLDivElement | null>(null);

    const stores = useContext(Context);

    const [connection, setConnection] = useState<signalR.HubConnection | null>(null)



    useEffect(() => {


        ChatApi.getAll()
            .then(res => setChats(res))
            .catch();


        setConnection(new signalR.HubConnectionBuilder()
            .withUrl(`${API_BASE}/hubs/chat?access_token=${localStorage.getItem("token")}`)
            .withAutomaticReconnect()
            .build());

        if (params.id !== undefined) {
            ChatApi.getId(parseInt(params.id))
                .then(res => {
                    if (res == 0)
                        ChatApi.add(params.id).then(() => ChatApi.getId(params.id).then(res => setChatId(res)))
                    else
                        setChatId(res);
                })
                .catch();
        }

    }, [])

    useEffect(() => {
        if (messagesDivRef.current) {
            setTimeout(() => {
                if (messagesDivRef != null && messagesDivRef.current != null) {
                    messagesDivRef.current.scrollTop = messagesDivRef.current.scrollHeight
                }
            }, 500);
        }
    }, [messages])

    useEffect(() => {
        setMessages([]);
        connection?.stop();

        setConnection(new signalR.HubConnectionBuilder()
            .withUrl(`${API_BASE}/hubs/chat?access_token=${localStorage.getItem("token")}`)
            .withAutomaticReconnect()
            .build());
    }, [chatId]);

    useEffect(() => {
        if (connection == null || chatId < 1)
            return;

        connection.on("ReceiveHistoryReact", (messagesHistory: MessageType[]) => {
            try {
                setMessages(messagesHistory)
            } catch (ex) { }

        })

        connection.on("ReceiveMessageReact", (message: MessageType) => {
            setMessages(prev => [...prev, message]);
        });

        connection.on("ReceiveImageReact", (message: MessageType) => {
            setMessages(prev => [...prev, message]);
        });

        connection.start().then(() => {
            connection.invoke("JoinChat", chatId);
        });
    }, [connection])

    const sendMessage = () => {
        if (message.length < 1 || connection == null || chatId < 1)
            return;

        connection.invoke("SendMessage", chatId, message);
        setMessage("");

    }

    return (

        <>
            <Container fluid className="vh-100">
                <Row className="h-100">
                    <Col xs={3} xxl={2} className="p-0">
                        <Sidebar />
                    </Col>

                    <Col xs={9} xxl={10} className="h-100">
                        <Row className="h-100">
                            <Col xs={4} xxl={3} className="border p-0 h-100 overflow-auto">
                                {chats.map((item, index) => <div key={index} onClick={() => setChatId(item.id)} className={`border p-3 ${item.id == chatId ? 'bg-dark text-light' : 'bg-light'}`} style={{ cursor: 'pointer' }}>
                                <div className="text-secondary small">Заказ № {item.orderId}</div>
                                <div className="text-secondary small">#{item.clientLogin}</div>
                                <div className="overflow-hidden" style={{ textWrap: 'nowrap' }}>{item.lastMessage}</div>
                                    <div className="text-secondary small">{(new Date(item.lastMessageDate).toLocaleDateString())} {(new Date(item.lastMessageDate).toLocaleTimeString())}</div>
                                </div>)}
                            </Col>
                            <Col xs={8} xxl={9} className="h-100 d-flex flex-column p-0">
                                <div className={`p-3 border ${chatId > 0 ? "d-flex" : "d-none"}`}>
                                    <div className="border rounded-circle fs-5 d-flex align-items-center justify-content-center" style={{ height: "42px", width: "42px" }}>
                                        <FaUser />
                                    </div>
                                    <div className="mx-2">
                                        <div className="small p-0">#{chats.find(c => c.id == chatId)?.clientLogin}</div>
                                        <div className="small p-0" onClick={() => setOrderShow(true)}>
                                            <a className="text-decoration-underline text-primary" style={{cursor: "pointer"}}>Заказ №{chats.find(c => c.id == chatId)?.orderId}</a>
                                        </div>
                                    </div>
                                </div>
                                <div className="flex-grow-1 overflow-auto p-3" ref={messagesDivRef}>
                                    {messages.map((item, index) => <div key={index} className={`border p-3 my-2 ${item.senderId == stores?.userStore.user?.id ? 'ms-auto' : ''}`} style={{ width: 'max-content' }}>
                                        <div className={`${item.senderId == stores?.userStore.user?.id ? 'd-none' : 'd-block text-secondary small'}`}>#{item.senderLogin}</div>
                                        {item.attachmentPath?.length < 5 ?
                                            <>
                                                <div>{item.content}</div>
                                            </>
                                            :
                                            <>
                                                <img src={`${API_BASE}${item.attachmentPath}`} style={{ maxHeight: '504px', maxWidth: '504px', objectFit: "cover" }} />
                                            </>
                                        }
                                        <div className="text-secondary small">{(new Date(item.createdAt).toLocaleDateString())} {(new Date(item.createdAt).toLocaleTimeString())}</div>

                                    </div>)}
                                </div>
                                <div className="d-flex p-3 border-top">
                                    <Form.Control
                                        placeholder="Введите сообщение"
                                        value={message}
                                        onChange={e => setMessage(e.currentTarget.value)}
                                    />
                                    <Button className="ms-2 d-flex align-items-center justify-content-center" onClick={() => {
                                        if (fileInputRef.current != null)
                                            fileInputRef.current.click()
                                    }}>
                                        <MdAttachFile />
                                    </Button>
                                    <Button className="ms-2 d-flex align-items-center justify-content-center" onClick={sendMessage}>
                                        <MdSend />
                                    </Button>
                                </div>

                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <input
                type="file"
                accept="image/*"
                ref={fileInputRef}
                style={{ display: "none" }}
                onChange={async (event) => {
                    const files = event.target.files;
                    if (files != null && files.length > 0 && chatId > 0) {
                        var path = await ChatApi.uploadImage(files[0], chatId);
                        if (path.length > 0)
                            connection?.invoke("SendImage", chatId, path)
                    }
                }}
            />

            <Modal show={orderShow} onHide={() => {
                setOrder(null);;
                setOrderShow(false)
            }} onShow={() => {
                if (chats.find(c => c.id == chatId) !== null)
                    OrderApi.get(chats.find(c => c.id == chatId)?.orderId ?? 0)
                        .then(res => setOrder(res))
                        .catch()
            }}>
                <Modal.Header closeButton>
                    <Modal.Title>Заказ №{order?.id}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div>Статус: {order?.status}</div>
                    <div>Цена доставки: {order?.deliveryPrice} ₽</div>
                    <div>Срок доставки: {order?.deliveryTime} мин.</div>
                    <div>Дата и время создания: {order == null ? 0 : getDateTime(order?.createdAt)}</div>
                    <div>Логин заказчика: {order?.clientLogin}</div>
                    <div>Итого: {order?.totalPrice} ₽</div>
                    <div>Продукты: </div>
                    <div className="overflow-auto" style={{ height: "120px" }}>
                        {
                            order?.products.map((product, i) => <div key={i} className="d-flex justify-content-between">
                                <div>{product.product.title}</div>
                                <div>x{product.count} | {product.product.price} ₽</div>
                            </div>)
                        }
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => {
                        setOrder(null);;
                        setOrderShow(false)
                    }}>
                        Закрыть
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}