import { Col, Container, Row } from "react-bootstrap";
import Sidebar from "../components/Sidebar";

export default function Home() {
    return (
        <>
            <Container fluid className="vh-100">
                <Row className="h-100">
                    <Col md={3} xxl={2} className="p-0">
                        <Sidebar />                        
                    </Col>

                    <Col md={9} xxl={10} className="p-4">
                        
                    </Col>
                </Row>
            </Container>
        </>
    )
}