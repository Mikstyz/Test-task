-- Создание бд
CREATE DATABASE dhub5
    WITH 
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TEMPLATE = template0;


-- Таблица продуктов
CREATE TABLE IF NOT EXISTS products
(
    id SERIAL PRIMARY KEY,
	seller TEXT NOT NULL,
    name TEXT NOT NULL,
    description TEXT,
    price NUMERIC(10,2) NOT NULL,
    quantity INT NOT NULL
);

CREATE INDEX idx_seller ON products (seller);
CREATE INDEX idx_name ON products (name);
CREATE INDEX idx_price ON products (price);
CREATE INDEX idx_quantity on products(quantity); 


-- Таблица пользователей
CREATE TABLE IF NOT EXISTS users
(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    user_login TEXT NOT NULL UNIQUE,
    user_password TEXT NOT NULL
);

-- Таблица заказов
CREATE TABLE IF NOT EXISTS orders
(
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE  -- Связь с пользователем
);

-- Таблица с таварми у заказа
CREATE TABLE IF NOT EXISTS order_products
(
    order_id INT REFERENCES orders(id) ON DELETE CASCADE,
    product_id INT REFERENCES products(id) ON DELETE CASCADE,
    PRIMARY KEY (order_id, product_id)
);
