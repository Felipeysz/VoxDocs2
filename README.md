# Sistema de Gerenciamento de Documentos

## Tecnologias Utilizadas
- ASP.NET Core MVC
- SQL Server
- Azure Blob Storage
- HTML/CSS com Bootstrap
- JavaScript

## Instalação
```bash
cd backend
dotnet restore
dotnet run
```

## Configuração
Configure o `appsettings.json` com:
```json
"AzureBlobStorage": {
  "ConnectionString": "<sua-chave>",
  "ContainerName": "documentos-container"
}
```

## Endpoints da API

### AuthController
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/Auth/Login | Faz login e retorna token |
| POST | /api/Auth/Register | Registra novo usuário |

### UserController
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | /api/User/GetUserBearerToken | Retorna dados do usuário autenticado |
| PUT | /api/User/UpdateUser | Atualiza nome ou senha |
| DELETE | /api/User/DeleteUser/{id} | Exclui usuário |

### TokenController
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/Token/Create | Cria novo token |
| GET | /api/Token/GetAll | Retorna todos os tokens |
| PUT | /api/Token/Renew/{id} | Renova validade do token |
| DELETE | /api/Token/Delete/{id} | Exclui token |

### DocumentoController
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/Documento/Upload | Upload de arquivo |
| GET | /api/Documento/Listar/{tipo} | Lista arquivos por tipo |

## Funcionalidades Futuras
- Permissões por token
- Compartilhamento de documentos
- Filtros por data e nome
- Estatísticas para admin