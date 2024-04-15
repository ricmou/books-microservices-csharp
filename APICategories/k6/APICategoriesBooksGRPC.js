import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:5501';

let client = new grpc.Client();
client.load(['../Protos'], 'APICategories.proto');


export default () => {
    client.connect(URL, {})

    let dataTagOnly =
    {
        Id: '978-0000000000'
    }

    let response = client.invoke('APICategoriesRPC.APICategoriesBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is not found': (r) => r && r.status === grpc.StatusNotFound,
    });

    let data = {
        CategoryId: 'TEG',
        Name: 'Tag1',
    };

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/AddNewCategory', data);
    check(response, {
        'status is ok Add 1': (r) => r.status === grpc.StatusOK,
    });

    data = {
        CategoryId: 'TE2',
        Name: 'Tag1',
    };

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/AddNewCategory', data);
    check(response, {
        'status is ok Add 2': (r) => r.status === grpc.StatusOK,
    });

    data = {
        Id: "978-0000000000",
        Categories: ["TEG"]    
    };

    response = client.invoke('APICategoriesRPC.APICategoriesBooksGRPC/AddNewBook', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    response = client.invoke('APICategoriesRPC.APICategoriesBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
        'Right isbn': (r) => r.message.Id === '978-0000000000',
        'Right Cat': (r) => r.message.Categories[0].CategoryId === 'TEG'
    });

    data = {
        Id: "978-0000000000",
        Categories: ["TE2"]    
    };

    response = client.invoke('APICategoriesRPC.APICategoriesBooksGRPC/ModifyBook', data);
    check(response, {
        'status is ok after changing Categories': (r) => r.status === grpc.StatusOK,
        'Categories change success': (r) => r.message.Categories[0].CategoryId === 'TE2'
    });

    response = client.invoke('APICategoriesRPC.APICategoriesBooksGRPC/DeleteBook', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'TEG'
    }

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/DeleteCategory', dataTagOnly);
    check(response, {
        'status is ok delete1': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'TE2'
    }

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/DeleteCategory', dataTagOnly);
    check(response, {
        'status is ok delete2': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}