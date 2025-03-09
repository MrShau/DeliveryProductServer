import React, { useEffect, useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { CategoryType, ProductType } from '../types';
import ProductApi from '../api/ProductApi';
import CategoryApi from '../api/CategoryApi';

interface UpdateProductModalProps {
    show: boolean;
    handleClose: () => void;
    productId: number
}

const UpdateProductModal: React.FC<UpdateProductModalProps> = ({ show, handleClose, productId }) => {
    const [product, setProduct] = useState<ProductType | null>(null);
    const [originalProduct, setOriginalProduct] = useState<ProductType | null>(null);
    const [image, setImage] = useState<File | null>(null);
    const [categories, setCategories] = useState<CategoryType[]>([]);


    useEffect(() => {
        if (productId > 0) {
            ProductApi.get(productId)
                .then(res => {
                    setProduct(res);
                    setOriginalProduct(res);
                })
                .catch();
            CategoryApi.getAll()
                .then(res => setCategories(res))
                .catch();
        }

    }, [productId])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setProduct(prevProduct => prevProduct ? { ...prevProduct, [name]: value } : null);
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            setImage(e.target.files[0]);
        }
    };

    const handleSubmit = async () => {
        if (originalProduct == null || product == null)
            return;

        if ((product?.title ?? '').length < 2)
            return alert("Введите название !");
        else
        {
            if (originalProduct?.title !== product?.title)
                await ProductApi.updateTitle(productId, product.title);
        }
        if ((product?.description ?? '').length < 3)
            return alert("Введите описание !")
        else 
        {
            if (originalProduct.description !== product.description)
                await ProductApi.updateDescription(productId, product.description);
        }
        if ((product?.price ?? 0) < 1)
            return alert("Введит цену !");
        else
        {
            if (originalProduct.price !== product.price)
                await ProductApi.updatePrice(productId, product.price);
        }
        if ((product?.categoryId ?? 0) < 1)
            return alert("Выберите категорию !");
        else 
        {
            if (originalProduct.categoryId !== product.categoryId)
                await ProductApi.updateCategory(productId, product.categoryId ?? 0);
        }

        if ((product?.count ?? 0) < 0)
            return alert("Введите количество");
        else 
        {
            if (originalProduct.count !== product.count)
                await ProductApi.updateCount(productId, product.count ?? 0);
        }

        if (image != null && image.size > 0)
            await ProductApi.updateImage(productId, image)

        handleClose();
    }

    return (
        <Modal show={show} onHide={() => {
            setProduct(originalProduct);
            handleClose();
        }}>
            <Modal.Header closeButton>
                <Modal.Title>Редактирование</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group>
                        <Form.Label>Название</Form.Label>
                        <Form.Control
                            type="text"
                            name="title"
                            value={product?.title ?? ''}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Описание</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            name="description"
                            value={product?.description ?? ''}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Категория</Form.Label>
                        <Form.Control
                            as="select"
                            name="categoryId"
                            value={product?.categoryId?.toString() ?? '0'}
                            onChange={handleChange}
                        >
                            <option value="0">Выберите категорию</option>
                            {categories.map((category, index) => (
                                <option key={index} value={category.id}>
                                    {category.name}
                                </option>
                            ))}
                        </Form.Control>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Цена</Form.Label>
                        <Form.Control
                            type="number"
                            name="price"
                            min="1"
                            value={product?.price ?? 0}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Количество</Form.Label>
                        <Form.Control
                            type="number"
                            name="count"
                            min="0"
                            value={product?.count ?? 0}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Файл изображения</Form.Label>
                        <Form.Control
                            type="file"
                            name="imageFile"
                            accept='image/*'
                            onChange={handleImageChange}
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" className='w-100' onClick={handleSubmit}>
                    Обновить
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default UpdateProductModal;
