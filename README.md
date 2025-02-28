# GenericMessageQueueSample

Bu proje, RabbitMQ ve Kafka gibi mesaj kuyruğu sağlayıcılarını kullanarak mesaj gönderme ve alma işlemlerini gerçekleştiren bir örnek uygulamadır.

## Gereksinimler

- .NET 9.0 SDK
- Docker ve Docker Compose

## Kurulum

1. Bu projeyi klonlayın:

    ```sh
    git clone https://github.com/kullaniciadi/GenericMessageQueueSample.git
    cd GenericMessageQueueSample
    ```

2. Docker kullanarak RabbitMQ ve Kafka servislerini başlatın:

    ```sh
    docker-compose up -d
    ```

3. Projeyi derleyin ve çalıştırın:

    ```sh
    dotnet build
    dotnet run
    ```

## Kullanım

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
Proje Yapısı
Program.cs: Uygulamanın giriş noktası.
IMessageQueueProvider.cs: Mesaj kuyruğu sağlayıcıları için arayüz.
RabbitMQProvider.cs: RabbitMQ sağlayıcısı.
KafkaProvider.cs: Kafka sağlayıcısı.
docker-compose.yml: RabbitMQ ve Kafka servislerini başlatmak için Docker Compose dosyası.
Katkıda Bulunma
Katkıda bulunmak isterseniz, lütfen bir pull request gönderin veya bir issue açın.
