import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import CategoryApi from '../api/CategoryApi';

interface ProductModalProps {
    show: boolean;
    categoryId: number;
    handleClose: () => void;
}

const ProductModal: React.FC<ProductModalProps> = ({ show, categoryId, handleClose }) => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [image, setImage] = useState<File | null>(null);
    const [price, setPrice] = useState(0);
    const [weight, setWeight] = useState(0);
    const [weightUnit, setWeightUnit] = useState("");
    const [count, setCount] = useState(0);

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            setImage(e.target.files[0]);
        }
    };

    const handleSubmit = async () => {
        if (name.length < 2)
            return alert("Введите название !");
        if (description.length < 3)
            return alert("Введите описание !");
        if (!image)
            return alert("Выберите изображение !");
        if (price < 1)
            return alert("Введите цену");
        if (weight < 1)
            return alert("Укажите граммовку")
        if (weightUnit.length < 2)
            return alert("Выберите единицу измерения");

        await CategoryApi.addProduct(categoryId, name, description, image, price, weight, weightUnit, count);

        setName('');
        setDescription('');
        setImage(null);
        setPrice(0);
        setWeight(0);
        setWeightUnit("");
        setCount(0);

        handleClose();
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Добавить товар</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group controlId="formProductName">
                        <Form.Label>Название</Form.Label>
                        <Form.Control
                            type="text"
                            placeholder="Введите название"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                        />
                    </Form.Group>
                    <Form.Group controlId="formProductDescription" className="mt-3">
                        <Form.Label>Описание</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            placeholder="Введите описание"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                        />
                    </Form.Group>
                    <Form.Group controlId="formProductImage" className="mt-3">
                        <Form.Label>Выбор изображения</Form.Label>
                        <Form.Control
                            type="file"
                            accept='image/*'
                            onChange={handleImageChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="formProductPrice" className="mt-3">
                        <Form.Label>Цена</Form.Label>
                        <Form.Control
                            type="number"
                            placeholder="Введите цену"
                            value={price}
                            onChange={(e) => setPrice(Number(e.target.value))}
                        />
                    </Form.Group>
                    <Form.Group controlId="formProductPrice" className="mt-3">
                        <Form.Label>Граммовка</Form.Label>
                        <Form.Control
                            type="number"
                            placeholder="Введите граммовку"
                            value={weight}
                            onChange={(e) => setWeight(Number(e.target.value))}
                        />
                    </Form.Group>

                    <Form.Group className="mt-3">
                        <Form.Label>Единица измерения</Form.Label>
                        <Form.Select onChange={e => setWeightUnit(e.target.value)}>
                            <option value="0">Выберите единицу измерения</option>
                            <option>г.</option>
                            <option>л.</option>
                            <option>шт.</option>
                        </Form.Select>
                    </Form.Group>
                </Form>

                <Form.Group controlId="formProductPrice" className="mt-3">
                        <Form.Label>Количество</Form.Label>
                        <Form.Control
                            type="number"
                            placeholder="Введите количество"
                            value={count}
                            onChange={(e) => setCount(Number(e.target.value))}
                        />
                    </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Закрыть
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    Сохранить
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ProductModal;