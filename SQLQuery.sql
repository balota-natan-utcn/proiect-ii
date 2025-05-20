USE DBProiectII;
GO

-- Create Customers table
CREATE TABLE Clienti (
    ID_Client INT PRIMARY KEY IDENTITY(1,1),
    Nume NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefon NVARCHAR(20)
);
GO

-- Create Products table
CREATE TABLE Produse (
    ID_Produs INT PRIMARY KEY IDENTITY(1,1),
    NumeProdus NVARCHAR(100) NOT NULL,
    Pret DECIMAL(10, 2) NOT NULL,
    Cantitate INT NOT NULL
);
GO

-- Create Orders table
CREATE TABLE Comenzi (
    ID_Comanda INT PRIMARY KEY IDENTITY(1,1),
    ID_Client INT FOREIGN KEY REFERENCES Clienti(ID_Client),
    DataComanda DATETIME DEFAULT GETDATE(),
    Total DECIMAL(10, 2) NOT NULL
);
GO

-- Insert sample data into Clienti
INSERT INTO Clienti (Nume, Email, Telefon)
VALUES 
    ('Octavian Cazan', 'octavian.cazan@email.com', '0743974199'),
    ('Geofri Suciu', 'geofri.suciu@email.com', '0741125662'),
    ('Bianca Surdu', 'bianca.surdu@email.com', '0745298755');
GO

-- Insert sample data into Produse
INSERT INTO Produse (NumeProdus, Pret, Cantitate)
VALUES 
    ('Laptop', 999.99, 10),
    ('Mouse', 24.99, 30),
    ('Tastatura', 49.99, 25),
    ('Monitor', 249.99, 15);
GO

-- Insert sample data into Comenzi
INSERT INTO Comenzi (ID_Client, DataComanda, Total)
VALUES 
    (1, '2025-01-15', 1049.98),
    (2, '2025-02-20', 299.98),
    (3, '2025-03-10', 999.99);
GO