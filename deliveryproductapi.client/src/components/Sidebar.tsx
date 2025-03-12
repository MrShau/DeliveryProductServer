import React, { useContext, useEffect, useState } from 'react';
import { Collapse, Button, Nav, Modal, Form } from 'react-bootstrap';
import { CategoryType } from '../types';
import CategoryApi from '../api/CategoryApi';
import { useNavigate } from 'react-router-dom';
import { FaPlus, FaSignOutAlt, FaUser } from 'react-icons/fa';
import { MdDelete } from 'react-icons/md';
import { Context } from '../main';
import UserApi from '../api/UserApi';

const Sidebar: React.FC = () => {
    const [openCategories, setOpenCategories] = useState(false);

    const [categories, setCategories] = useState<CategoryType[]>([]);

    const [categoryName, setCategoryName] = useState('');
    const [showAddCategory, setShowAddCategory] = useState(false);

    const navigate = useNavigate();

    const stores = useContext(Context)

    useEffect(() => {
        CategoryApi.getAll()
            .then(res => setCategories(res))
            .catch();
    }, []);

    return (
        <>
            <div className="d-flex flex-column flex-shrink-0 vh-100 bg-dark text-light overflow-auto">
                <div className="d-flex align-items-center pb-3 mb-3 link-body-emphasis text-decoration-none border-bottom">
                    <span className="fs-5 fw-semibold mx-auto mt-4 text-light">Админ-Панель</span>
                </div>
                <Nav className="list-unstyled ps-0 flex-column">
                    <li className="mb-1 w-100">
                        <Button
                            variant="dark"
                            className="btn btn-toggle w-100 d-inline-flex align-items-center rounded-0 py-2 border-0"
                            onClick={() => setOpenCategories(!openCategories)}
                            aria-expanded={openCategories ? 'true' : 'false'}
                        >
                            <div className="d-flex w-100 justify-content-between align-items-center">
                                <div>Категории</div>
                                <span
                                    className="btn btn-sm btn-primary rounded-circle p-1 d-flex justify-content-center align-items-center"
                                    style={{ cursor: 'pointer', width: '32px', height: '32px' }}
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        setShowAddCategory(true);
                                    }}
                                >
                                    <FaPlus style={{ fontSize: "10px" }} />
                                </span>
                            </div>
                        </Button>
                        <Collapse in={openCategories}>
                            <div className="fw-normal pb-1 small w-100 border-bottom border-top">
                                {categories.map((category, index) => <Button key={index} className='rounded-0 w-100 text-start btn-dark' onClick={() => navigate(`/category/${category.id}`)}><div className="d-flex w-100 justify-content-between align-items-center">
                                <div>{category.name}</div>
                                <span
                                    className="btn btn-sm btn-danger rounded-circle p-1 d-flex justify-content-center align-items-center"
                                    style={{ cursor: 'pointer', minWidth: '32px', minHeight: '32px', maxHeight: '32px', maxWidth: '32px' }}
                                    onClick={async (e) => {
                                        e.stopPropagation();
                                        if (confirm(`Вы уверены, что хотите удалить категорию: ${category.name}\nПеред удалением категории вы можете перенести продукты в другую категорию, чтобы продукты не удалились`))
                                        {
                                            await CategoryApi.delete(category.id);
                                            setCategories(await CategoryApi.getAll());
                                        }
                                    }}
                                >
                                    <MdDelete style={{ fontSize: "16px" }} />
                                </span>
                            </div></Button>)}
                            </div>
                        </Collapse>
                    </li>

                    <Button
                        variant='dark'
                        className="mb-1 btn btn-toggle w-100 d-inline-flex align-items-center rounded-0 py-2 border-0"
                        onClick={() => navigate("/orders")}
                        style={{height: "48px"}}
                    >
                        Заказы
                    </Button>

                    <Button
                        variant='dark'
                        className="mb-1 btn btn-toggle w-100 d-inline-flex align-items-center rounded-0 py-2 border-0"
                        onClick={() => navigate("/chats")}
                        style={{height: "48px"}}
                    >
                        Чаты
                    </Button>
                </Nav>
                <div className='d-flex mt-auto p-3 border-top' style={{justifyContent: "space-between"}}>
                    <div className='d-flex'>
                    <div
                        className='d-flex align-items-center justify-content-center rounded-circle fs-2 text-center'
                        style={{
                            width: '52px', 
                            height: '52px',
                            background: 'whitesmoke',
                            color: 'black',
                            }}>
                            <FaUser />
                    </div>
                    <div className='d-flex align-items-center px-2'>
                        <div>
                            <span className='ellipsis-text small text-secondary d-block'>#{stores?.userStore.user?.login ?? ""}</span>
                            <span className='ellipsis-text small text-secondary d-block'>{stores?.userStore.user?.role}</span>
                        
                        </div>
                    </div>
                    </div>
                    
                    <div className='d-flex align-items-center'>
                        <Button variant='outline-primary' onClick={() => {
                            if (confirm("Вы уверены, что хотите выйти из этого аккаунта ?"))
                                UserApi.signOut()
                        }}>
                            <FaSignOutAlt />
                        </Button>
                    </div>
                </div>
            </div>
            <Modal show={showAddCategory} onHide={() => setShowAddCategory(false)} centered>
            <Modal.Header closeButton>
                <Modal.Title>Добавить категорию</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group>
                        <Form.Label>Название категории</Form.Label>
                        <Form.Control
                            type="text"
                            value={categoryName}
                            onChange={(e) => setCategoryName(e.target.value)}
                            placeholder="Введите название"
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={() => {
                    setCategoryName("")
                    setShowAddCategory(false)
                }}>
                    Отмена
                </Button>
                <Button variant="primary" onClick={async () => {
                    if (categoryName.length < 2)
                        return alert("Введите название категории !");
                    await CategoryApi.add(categoryName).then(async () => {setCategories(await CategoryApi.getAll()); setCategoryName("")});
                    setShowAddCategory(false)
                }}>
                    Добавить
                </Button>
            </Modal.Footer>
        </Modal>
        </>
    );
};

export default Sidebar;
