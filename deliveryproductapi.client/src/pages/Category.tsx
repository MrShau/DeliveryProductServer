import { Button, Card, Col, Container, Row } from "react-bootstrap";
import Sidebar from "../components/Sidebar";
import { useParams } from "react-router-dom";
import { FaPlus } from "react-icons/fa";
import { useEffect, useState } from "react";
import ProductModal from "../components/AddProductModal";
import { ProductType } from "../types";
import CategoryApi from "../api/CategoryApi";
import { API_BASE } from "../consts";
import { MdDelete, MdEdit } from "react-icons/md";
import UpdateProductModal from "../components/UpdateProductModal";
import ProductApi from "../api/ProductApi";

export default function Category() {
    const { id } = useParams();
    if (id == null || Number.parseInt(id) < 1)
        return (<>Указана неправильная категория !</>)

    const [products, setProducts] = useState<ProductType[]>([]);

    const [showModal, setShowModal] = useState(false);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [selectedProductId, setSelectedProductId] = useState(0);

    const [categoryName, setCategoryName] = useState("...");

    useEffect(() => {
        CategoryApi.get(Number.parseInt(id))
            .then(res => setCategoryName(res?.name ?? "..."))
            .catch();

        CategoryApi.getProducts(Number.parseInt(id))
            .then(res => setProducts(res))
            .catch();
    }, [id]);

    return (
        <>
            <Container fluid className="vh-100">
            <Row className="h-100">
                <Col xs={3} xxl={2} className="p-0">
                <Sidebar />                        
                </Col>

                <Col xs={9} xxl={10} className="h-100 p-4 overflow-auto">
                <div className="d-flex justify-content-between mb-3">
                <h4 className="mb-3">Список продуктов категории: {categoryName}</h4>
                <Button className="px-3" onClick={() => setShowModal(true)}><FaPlus /></Button>
                </div>
                <Row xs={1} md={2} lg={3} xl={4} xxl={5} className="my-2 g-4 overflow-auto">
                    {products.map((product, index) => (
                    <Col key={index}>
                        <Card style={{height: "302px"}}>
                        <div className="position-relative">
                            <Card.Img variant="top" src={`${API_BASE}${product.imagePath}`} style={{height: "152px", objectFit: "cover"}} />
                            <div className="position-absolute top-0 p-1" style={{right: 0, background: "rgba(0, 0, 0, .5)"}}>
                            <Button className="rounded-0 border-0" variant="outline-primary" onClick={() => {
                                setSelectedProductId(product.id);
                                setShowUpdateModal(true);
                            }}><MdEdit /></Button>
                            <Button className="rounded-0 border-0" variant="outline-danger" onClick={async () => {
                                if (confirm("Вы уверены, что хотите удалить продукт " + product.title + " ? ")) {
                                    await ProductApi.delete(product.id)
                                    CategoryApi.getProducts(Number.parseInt(id))
                                    .then(res => setProducts(res))
                                    .catch();
                                }
                            }}><MdDelete /></Button>
                            </div>
                        </div>
                        <Card.Body className="p-2">
                            <Card.Title className="ellipsis-text my-1">{product.title}</Card.Title>
                            <div className="text-secondary">Цена: {product.price} ₽</div>
                            <div className="text-secondary">Граммовка: {product.weight} {product.weightUnit}</div>
                            <div className="text-secondary">В наличии: {product.count}</div>
                            <Card.Text className="text-secondary ellipsis-text">Описание: {product.description}</Card.Text>
                        </Card.Body>
                        </Card>
                    </Col>
                    ))}
                   
                </Row>

                </Col>
            </Row>
            </Container>
            <ProductModal show={showModal} categoryId={Number.parseInt(id)} handleClose={() => {
                setShowModal(false)
                CategoryApi.getProducts(Number.parseInt(id))
                .then(res => setProducts(res))
            }} />
            <UpdateProductModal show={showUpdateModal} productId={selectedProductId} handleClose={() => {
                setShowUpdateModal(false)
                CategoryApi.getProducts(Number.parseInt(id))
            .then(res => setProducts(res))
            .catch();
            }} />
        </>
    )
}