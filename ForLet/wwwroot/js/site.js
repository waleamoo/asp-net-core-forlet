// Jquery for the web app
$(document).ready(function () {

    // for dynamic state drop down list  
    $('.dynamic-state').change(function () {
        if ($(this).val() != '') {
            var stateId = $(this).val(); // gets stateId
            var dependent = $(this).data('dependent'); // lga (for the id of the dependent drop down) 
            //console.log(stateId, " ", dependent);
            $.ajax({
                url: "/home/getlga",
                method: "GET",
                data: {
                    stateId: stateId,
                    dependent: dependent
                },
                success: function (result) {
                    $('#' + dependent).html(result);
                }
            })
        }
    });

    //alert('Hello Wale');
    $("#Agreement").hide();
    $("#Damages").hide();
    // when selection changes 
    $("#agreement-select").change(function () {
        if ($(this).val() != "") {
            var val = $(this).val();
            if (val === "Yes") {
                $("#Agreement").show();

            } else {
                $("#Agreement").hide();
            }
        }
    });

    $("#damages-select").change(function () {
        if ($(this).val() != "") {
            var val = $(this).val();
            if (val === "Yes") {
                $("#Damages").show();
            } else {
                $("#Damages").hide();
            }
        }
    });
    // input mask section 
    $("#PhoneNumber").inputmask({ "mask": "9{1,11}" });
    $("#FullName").inputmask({ "mask": "a{1,30} a{1,30}" });
    $("#LastName").inputmask({ "mask": "a{1,30}" });
    $("#FirstName").inputmask({ "mask": "a{1,30}" });
    //$("#AreaName").inputmask({ "mask": "a{1,50}, a{1,50}" });
    $("#HouseNumber").inputmask({ "mask": "9{1,4}" });
    //$("#StreetName").inputmask({ "mask": "*{1,50} Street" });

    // Rental Image Upload 
    function readURL(input) {
        if (input.files && input.files[0]) {
            let reader = new FileReader();
            reader.onload = function (e) {
                $("img#imgUpload").attr("src", e.target.result).width(600).height(300);
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
    $("#ImageUpload").change(function () {
        readURL(this);
    });

    // profile Image Upload
    function readProfileURL(input) {
        if (input.files && input.files[0]) {
            let reader = new FileReader();
            reader.onload = function (e) {
                $("img#avatarUpload").attr("src", e.target.result).width(200).height(200);
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
    $("#ProfileImage").change(function () {
        readProfileURL(this);
    });

    // timeout alert 
    if ($("div.alert.notification").length) {
        setTimeout(() => {
            $("div.alert.notification").fadeOut();
        }, 4000);
    }

    // role delete confirmation 
    if ($("a.confirmDeletion").length) {
        $("a.confirmDeletion").click(() => {
            if (!confirm("Confirm deletion")) {
                return false;
            }
        })
    }

    // auto generate fields 
    $(document).on("blur", "#PhoneNumber", function () {
        let userNumber = $(this).val();
        //alert(userNumber);
        $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            url: "/home/getUser",
            method: "GET",
            data: {userNumber: userNumber},
            dataType: 'json',
            success: function (response) {
                if (response.msg === "success") {
                    $('#FullName').val(response.fullName);
                    $('#Email').val(response.email);
                    //$("#FullName").attr("readonly", true);
                    //$("#Email").attr("readonly", true);
                } else {
                    console.log(response.msg);
                }
            }
        })
    });

});

