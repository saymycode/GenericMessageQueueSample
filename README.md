# GenericMessageQueueSample

This project demonstrates how to send and receive messages using message queue providers like RabbitMQ and Kafka.

Bu proje, RabbitMQ ve Kafka gibi mesaj kuyruğu sağlayıcılarını kullanarak mesaj gönderme ve alma işlemlerini gerçekleştiren bir örnek uygulamadır.

## Requirements

- .NET 9.0 SDK
- Docker and Docker Compose

## Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/kullaniciadi/GenericMessageQueueSample.git
    cd GenericMessageQueueSample
    ```

    1. Repo'yu klonlayın:

    ```sh
    git clone https://github.com/kullaniciadi/GenericMessageQueueSample.git
    cd GenericMessageQueueSample
    ```

2. Start RabbitMQ and Kafka services using Docker:

    ```sh
    docker-compose up -d
    ```

    2. Docker kullanarak RabbitMQ ve Kafka servislerini başlatın:

    ```sh
    docker-compose up -d
    ```

3. Build and run the project:

    ```sh
    dotnet build
    dotnet run
    ```

    3. Projeyi derleyin ve çalıştırın:

    ```sh
    dotnet build
    dotnet run
    ```

## Usage

When the application runs, messages are sent and received using RabbitMQ and Kafka. The console will show outputs like the following:

Uygulama çalıştırıldığında, RabbitMQ ve Kafka sağlayıcıları kullanılarak mesajlar gönderilir ve alınır. Konsolda aşağıdaki gibi çıktılar görünecektir:

```plaintext
RabbitMQ: Sent Hello from RabbitMQ! at 2023-10-01T12:00:00.000Z
RabbitMQ: Received Hello from RabbitMQ! at 2023-10-01T12:00:01.000Z
Elapsed time: 1000 ms
Kafka: Sent Hello from Kafka! at 2023-10-01T12:00:00.000Z
Kafka: Received Hello from Kafka! at 2023-10-01T12:00:01.000Z
Elapsed time: 1000 ms
Press [enter] to exit.
```

## Project Structure

- `Program.cs`: The entry point of the application.
- `Interfaces/IMessageQueueProvider.cs`: Interface for message queue providers.
- `Providers/RabbitMQProvider.cs`: RabbitMQ provider.
- `Providers/KafkaProvider.cs`: Kafka provider.
- `docker-compose.yml`: Docker Compose file to start RabbitMQ and Kafka services.

Proje Yapısı

- `Program.cs`: Uygulamanın giriş noktası.
- `Interfaces/IMessageQueueProvider.cs`: Mesaj kuyruğu sağlayıcıları için arayüz.
- `Providers/RabbitMQProvider.cs`: RabbitMQ sağlayıcısı.
- `Providers/KafkaProvider.cs`: Kafka sağlayıcısı.
- `docker-compose.yml`: RabbitMQ ve Kafka servislerini başlatmak için Docker Compose dosyası.

## Contributing

If you'd like to contribute, please submit a pull request or open an issue.

Katkıda bulunmak isterseniz, lütfen bir pull request gönderin veya bir issue açın.

---
