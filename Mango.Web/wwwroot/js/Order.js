﻿var datatable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDatatable("approved")
    } else {
        if (url.includes("readyforpickup")) {
            loadDatatable("readyforpickup")
        } else {
            if (url.includes("cancelled")) {
                loadDatatable("cancelled")
            } else {
                loadDatatable("all")
            }
        }
    }
});

function loadDatatable(status) {
    datatable = $("#tblData").DataTable({
        "ajax": { url: "/order/getall?status=" + status },
        "columns": [
            {
                data:'orderHeaderId',"width":"5%"
            },
            {
                data: 'email', "width": "25%"
            },
            {
                data: 'firstname', "width": "20%"
            },
            {
                data: 'phone', "width": "10%"
            },
            {
                data: 'status', "width": "10%"
            },
            {
                data: 'orderTotal', "width": "10%"
            }, {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/order/orderDetails?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width": "10%"
            }
        ]
    })
}