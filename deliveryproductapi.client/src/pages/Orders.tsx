import { Button, Col, Container, Row } from "react-bootstrap";
import Sidebar from "../components/Sidebar";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { OrderType } from "../types";
import OrderApi from "../api/OrderApi";
import { getDateTime } from "../consts";
import { MdMessage } from "react-icons/md";

export default function Orders() {

    const [tab, setTab] = useState(1);

    const [orders, setOrders] = useState<OrderType[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        setOrders([]);
        if (tab == 1)
        OrderApi.getWaitConfirmations()
            .then(res => setOrders(res))
            .catch();
            else if (tab == 2)
                OrderApi.getActives()
            .then(res => setOrders(res))
            .catch();
            else if (tab == 3)
                OrderApi.getCompleted()
            .then(res => setOrders(res))
            .catch();
    }, [tab])
    

    return (
        <>
        <Container fluid className="vh-100">
                <Row className="h-100">
                    <Col xs={3} xxl={2} className="p-0">
                        <Sidebar />                        
                    </Col>

                    <Col xs={9} xxl={10} className="p-4 h-100 overflow-auto">
                        <h4 className="mb-3">Список заказов</h4>
                        <div className="d-flex">
                            <Button className="rounded-0 me-2" onClick={() => setTab(1)} variant={`${tab == 1 ? 'primary' : 'outline-primary'}`}>
                                Ожидающие подтверждения
                            </Button>
                            <Button className="rounded-0 me-2" onClick={() => setTab(2)} variant={`${tab == 2 ? 'primary' : 'outline-primary'}`}>
                                В процессе
                            </Button>
                            <Button className="rounded-0" onClick={() => setTab(3)} variant={`${tab == 3 ? 'primary' : 'outline-primary'}`}>
                                Завершенные
                            </Button>
                        </div>

                        <Row xs={1} md={2} xl={3} xxl={4} className="my-3 mx-0">
                            {orders.map((item, index) => <Col key={index} className="p-2 ps-0 pt-0 overflow-hidden " style={{height: `${tab == 1 ? '460px' : '400px'}`}}>
                            <div className="bg-body p-3 h-100 rounded-2 position-relative">
                                <div className="position-absolute top-0 p-3" style={{right: 0}}>
                                    <Button onClick={() => navigate(`/chats/${item.id}`)} className="rounded-circle d-flex align-items-center justify-content-center p-0" style={{height: '32px', width: '32px'}}>
                                        <MdMessage />
                                    </Button>
                                </div>
                            <h5>Заказ №{item.id}</h5>
                                <div>Статус: {item.status}</div>
                                <div>Цена доставки: {item.deliveryPrice} ₽</div>
                                    <div>Срок доставки: {item.deliveryTime} мин.</div>
                                    <div>Адрес: { item.address}</div>
                                <div>Дата и время создания: {getDateTime(item.createdAt)}</div>
                                <div>Логин заказчика: {item.clientLogin}</div>
                                <div>Итого: {item.totalPrice} ₽</div>
                                <div>Продукты: </div>
                                <div className="overflow-auto" style={{height: "120px"}}>
                                {
                                    item.products.map((product, i) => <div key={i} className="d-flex justify-content-between">
                                        <div>{product.product.title}</div>
                                        <div>x{product.count} | {product.product.price} ₽</div>
                                        </div>)
                                }
                                </div>
                                <Button variant="success" className={`my-2 w-100 ${tab != 1 ? 'd-none' : 'd-block'}`} onClick={async() => {
                                    await OrderApi.confirm(item.id);
                                }}>
                                    Подтвердить
                                </Button>
                            </div>
                                
                            </Col>)}
                        </Row>
                    </Col>
                </Row>
            </Container>
        </>
    )
}