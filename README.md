# Bookmania
O sistema simula uma livraria online com controle de acesso baseado em papéis e fluxo completo de pedidos.

## Tecnologias
- ⚙️ ASP.NET Core
- 🧩 Razor Pages
- 🔐 ASP.NET Core Identity
- 🗄️ Entity Framework Core
- 🛢️ Microsoft SQL Server
- 🎨 HTML5, CSS3, JavaScript
- 🅱️ Bootstrap

## Funcionalidade por Perfil
- 🛍️ Cliente
  - Registro e login
  - Visualização do catálogo de livros
  - Adição de livros ao carrinho
  - Escolha entre compra ou aluguel
  - Geração de pedidos
  - Acompanhamento de status
- 🧾 Funcionário
  - Visualização de pedidos gerados
  - Confirmação de pagamento
  - CRUD de livros
  - CRUD de usuários (com restrições)
- 📊 Gerente
  - Mesmas permissões do funcionário
  - Menos restrições administrativas
- 🛠️ Administrador
  - Acesso total ao sistema
  - Sem restrições de edição ou visualização
 
  ## Como executar
  1️⃣ Clone o repositório
  `git clone https://github.com/dev-pedr0/Bookmania`
  2️⃣ Configure a string de conexão
  No arquivo appsettings.json, configure a conexão com o SQL Server:
  `"ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=BookmaniaDB;Trusted_Connection=True;"
  }`
  3️⃣ Execute as migrations
  `dotnet ef database update`
  4️⃣ Execute o projeto
  `dotnet run`
