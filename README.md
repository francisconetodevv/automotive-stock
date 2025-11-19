# Automotive Stock Management System

A distributed system for real-time stock synchronization across multiple manufacturing plants for Automotive Manufacturing Corp.

## 🎯 Project Overview

The Automotive Stock Management System is designed to synchronize inventory across three manufacturing plants in Brazil, providing real-time visibility of stock levels, automated purchase orders, and intelligent stock management.

### Plants Overview

- **Plant A (São Paulo)** - Motors
  - Products: Engine blocks, cylinder heads, pistons
  - Main materials: Special steel, cast aluminum, bronze

- **Plant B (Minas Gerais)** - Chassis
  - Products: Chassis, structures, suspensions
  - Main materials: Carbon steel, stainless steel, rubber

- **Plant C (Rio de Janeiro)** - Plastic Components
  - Products: Bumpers, interior panels, coverings
  - Main materials: Polypropylene, ABS, PVC

## 🚀 Key Features

- Real-time material consumption tracking
- Centralized stock monitoring
- Automatic low-stock alerts
- Automated purchase order generation
- Inter-plant stock notifications

## 🛠️ Technical Stack

- **Framework:** .NET 9.0
- **Language:** C#
- **Message Broker:** RabbitMQ
- **Message Format:** JSON

## 📦 Solution Structure

```
AutomotiveStock.sln
├── src/
│   ├── AutomotiveStock.CentralStock/       # Central stock management system
│   ├── AutomotiveStock.PlantSimulator/     # Plant simulation system
│   ├── AutomotiveStock.Purchasing/         # Purchasing management system
│   └── AutomotiveStock.Shared/             # Shared libraries and models
```

## 🏗️ Architecture

### RabbitMQ Exchange and Queues

```
Exchange: stock.events (topic)
├── Routing Keys:
│   ├── consumption.{plant}
│   ├── replenishment.{plant}
│   ├── alert.lowstock
│   └── purchase.order
│
├── Queues:
    ├── queue.central.stock         # Consumes all events
    ├── queue.purchasing.alerts     # Consumes only alerts
    ├── queue.plant.a.notifications # Plant A notifications
    ├── queue.plant.b.notifications # Plant B notifications
    └── queue.plant.c.notifications # Plant C notifications
```

## 🔄 Main Workflows

1. **Material Consumption Flow**
   - Production order consumes material
   - Plant system publishes consumption event
   - Central system updates stock
   - Low stock alerts triggered if necessary

2. **Replenishment Flow**
   - Plant receives material
   - System publishes replenishment event
   - Central stock updated
   - Other plants notified

3. **Automatic Purchase Order Flow**
   - System detects critical stock
   - Calculates required quantity
   - Generates purchase order
   - Notifies purchasing department

## 📋 Development Phases

### Phase 1 - MVP
- [ ] Console applications for three plants
- [ ] Central stock console application
- [ ] RabbitMQ configuration
- [ ] Basic event publishing/consuming
- [ ] Structured console logging

### Phase 2 - Core Features
- [ ] Minimum stock calculations
- [ ] Alert generation
- [ ] Automatic purchase orders
- [ ] Inter-plant notifications

### Phase 3 - Improvements
- [ ] Database persistence
- [ ] REST API
- [ ] Web dashboard
- [ ] Unit tests

## ⚙️ Configuration

### Material Classification
- **Class A:** Critical materials - automatic orders
- **Class B:** Important materials - alerts only
- **Class C:** Common materials - notifications only

### Lead Times
- Steel: 15 business days
- Aluminum: 20 business days
- Plastic: 10 business days
- Rubber: 12 business days
- Electronics: 30 business days

## 🧪 Testing

The system includes predefined test scenarios:

1. **Normal Consumption**
   - Multiple consumption events
   - Stock validation

2. **Critical Stock**
   - Alert triggering
   - Purchase order generation

3. **Replenishment**
   - Stock updates
   - Inter-plant notifications

## 🚦 Acceptance Criteria

1. Process 50 consecutive consumption events without errors
2. Generate alerts within 3 seconds of reaching minimum stock
3. Correctly calculate purchase order quantities
4. Message persistence during RabbitMQ outages
5. Complete event traceability

## 🔧 Setup Instructions

1. Clone the repository
2. Set up RabbitMQ (Docker recommended)
3. Configure connection strings
4. Build and run the solution

## 📚 Documentation

For detailed information about:
- Business rules
- Event structures
- API endpoints
- Database schemas

Please refer to the project wiki.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request



## 📞 Support

For support and queries, please contact the development team.

---

Generated: October 29, 2025

© 2025 Automotive Manufacturing Corp. All rights reserved.