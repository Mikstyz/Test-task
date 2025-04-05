-- Таблица пользователей
CREATE TABLE IF NOT EXISTS users
(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    user_login TEXT NOT NULL UNIQUE,
    user_password TEXT NOT NULL
);

-- Таблица продуктов
CREATE TABLE products
(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT,
    price NUMERIC(10,2) NOT NULL,
    quantity INT NOT NULL,
    seller INT NOT NULL REFERENCES users(id) ON DELETE CASCADE
);

-- Таблица заказов
CREATE TABLE IF NOT EXISTS orders
(
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE
);

-- Таблица с товарами у заказа
CREATE TABLE IF NOT EXISTS order_products
(
    order_id INT REFERENCES orders(id) ON DELETE CASCADE,
    product_id INT REFERENCES products(id) ON DELETE CASCADE,
    product_quantity INT,
    PRIMARY KEY (order_id, product_id)
);

-- Индекс на user_id в orders
CREATE INDEX idx_orders_user_id ON orders(user_id);

-- Индекс на seller в products
CREATE INDEX idx_products_seller ON products(seller);

-- Индексы на order_products
CREATE INDEX idx_order_products_product_id ON order_products(product_id);
CREATE INDEX idx_order_products_order_id ON order_products(order_id);