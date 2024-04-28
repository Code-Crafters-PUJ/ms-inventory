-- Insertar datos en SupplierType
INSERT INTO SupplierType (SupplierTypeId, Name) VALUES
(1, 'Proveedor de productos');

-- Insertar datos en ServiceType
INSERT INTO ServiceType (ServiceTypeId, Name) VALUES
(1, 'Proveedor de servicios');

-- Insertar datos en Supplier
INSERT INTO Supplier (SupplierId, Name, Address, Phone, Email, UrlPage, SupplierTypeId, ServiceTypeId) VALUES
(1, 'Proveedor A', '123 Calle Principal', '123456789', 'proveedora@example.com', 'http://www.proveedorA.com', 1, 1),
(2, 'Proveedor B', '456 Avenida Secundaria', '987654321', 'proveedorb@example.com', 'http://www.proveedorB.com', 1, 1);
