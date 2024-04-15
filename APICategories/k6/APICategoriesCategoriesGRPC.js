import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:5501';

let client = new grpc.Client();
client.load(['../Protos'], 'APICategories.proto');


export default () => {
    client.connect(URL, {})
    
    let dataTagOnly =
    {
        Id: 'TEG'
    }

    let response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/GetCategoryByID', dataTagOnly);
    check(response, {
        'status is not found': (r) => r && r.status === grpc.StatusNotFound,
    });

    let data = {
        CategoryId: 'TEG',
        Name: 'Tag1',
    };

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/AddNewCategory', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/GetCategoryByID', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
    });

    data = {
        CategoryId: 'TEG',
        Name: 'Tag2',
    };

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/ModifyCategory', data);
    check(response, {
        'status is ok after changing Name': (r) => r.status === grpc.StatusOK,
        'Name change success': (r) => r.message.Name === 'Tag2'
    });

    response = client.invoke('APICategoriesRPC.APICategoriesCategoryGRPC/DeleteCategory', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}