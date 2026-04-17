-- Rooms
INSERT INTO Room (Number, Name, Description) VALUES
    ('101', 'Серверная',        'Помещение серверного оборудования'),
    ('202', 'Бухгалтерия',      NULL),
    ('305', 'Отдел разработки', 'Открытое рабочее пространство'),
    ('401', 'Переговорная',     'Конференц-зал');

-- Employees
INSERT INTO Employee (FullName, Position, Department) VALUES
    ('Иванов Иван Иванович',       'Системный администратор', 'IT-отдел'),
    ('Петрова Мария Сергеевна',    'Главный бухгалтер',       'Бухгалтерия'),
    ('Сидоров Алексей Владимирович','Руководитель проекта',    'Отдел разработки'),
    ('Кузнецова Ольга Николаевна', 'Разработчик',             'Отдел разработки');

-- Equipment
-- Status: 1=Active, 2=UnderRepair, 3=WrittenOff, 4=Reserved
INSERT INTO Equipment (Name, InventoryNumber, SerialNumber, Status, RoomId, EmployeeId, Notes) VALUES
    ('Сервер Dell PowerEdge R740', 'INV-0001', 'SN-DELL-001', 1, 1, 1, 'Основной сервер'),
    ('ИБП APC Smart-UPS 1500',    'INV-0002', 'SN-APC-001',  1, 1, 1, NULL),
    ('ПК HP EliteDesk 800 G6',    'INV-0003', 'SN-HP-0003',  1, 2, 2, NULL),
    ('Монитор LG 27UK850',        'INV-0004', 'SN-LG-0004',  1, 2, 2, NULL),
    ('Ноутбук Lenovo ThinkPad X1','INV-0005', 'SN-LEN-005',  1, 3, 3, 'Руководитель проекта'),
    ('Ноутбук Dell Latitude 5520','INV-0006', 'SN-DELL-006', 1, 3, 4, NULL),
    ('Принтер HP LaserJet Pro',   'INV-0007', 'SN-HP-0007',  2, 3, NULL, 'На ремонте'),
    ('Проектор Epson EB-X41',     'INV-0008', 'SN-EPS-008',  4, 4, NULL, NULL);

-- Inventory Check
INSERT INTO InventoryCheck (CheckDate, ResponsibleEmployeeId, Comment) VALUES
    ('2024-10-01', 1, 'Плановая инвентаризация за IV квартал 2024 года');

-- Inventory Check Items (CheckId=1)
-- ResultStatus: 1=Present, 2=Missing, 3=Damaged, 4=Surplus
INSERT INTO InventoryCheckItem (InventoryCheckId, EquipmentId, ResultStatus, Comment) VALUES
    (1, 1, 1, NULL),
    (1, 2, 1, NULL),
    (1, 3, 1, NULL),
    (1, 4, 1, NULL),
    (1, 5, 1, NULL),
    (1, 6, 1, NULL),
    (1, 7, 3, 'Неисправность блока питания, направлен в ремонт'),
    (1, 8, 1, NULL);

-- Movement History
INSERT INTO MovementHistory (EquipmentId, FromRoomId, ToRoomId, FromEmployeeId, ToEmployeeId, MovedAt, Comment) VALUES
    (5, 2, 3, 2, 3, '2024-03-15', 'Передача ноутбука новому руководителю проекта'),
    (7, 2, 3, 2, NULL, '2024-09-10', 'Перемещение принтера в отдел разработки'),
    (8, 3, 4, NULL, NULL, '2024-08-01', 'Перемещение проектора в переговорную');
