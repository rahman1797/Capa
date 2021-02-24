$(document).ready(function () {
        var table = $('#datatable-fixed-col').DataTable({
            scrollY: "400px",
            scrollX: true,
            scrollCollapse: true,
            fixedColumns: {
                leftColumns: 3,
                //rightColumns: 1
            },
            paging: false,
            ordering: false,
            searching: false,
            info: false, 
        });
    });

    $(".checkAll").click(function(){

        var len = $("[name='no_applicant[]']:checked").length;
        var max = $("#max_komparasi_pelamar").val();

        if(len > 0)
            $("#btnCompare").removeAttr('disabled');
        else
            $("#btnCompare").attr('disabled','disabled');

        if(len > max)
        {
            swal({
                title: "Perhatian!",
                text: "Jumlah Pelamar yang dipilih maksimal "+max+" orang",
                type: "warning",
                confirmButtonColor: "#EF5350",
                confirmButtonText: "OK",
                cancelButtonText: "Batal"
            });
            return false;
        }

        $("#jmlTerpilih").text(len);
    });

    function comparePelamar()
    {
        var arrApplicant = $("[name='no_applicant[]']:checked").map(function(){
          return $(this).val();
        }).get();

        var len = $("[name='no_applicant[]']:checked").length;

        if(len == 0)
        {
            swal({
                title: "Perhatian!",
                text: "Pelamar belum ada yang dipilih!",
                type: "warning",
                confirmButtonColor: "#EF5350",
                confirmButtonText: "OK",
                cancelButtonText: "Batal"
            });
            return false;
        }

        $("#divComparison").empty();
        $("#divSpinner").show();
        $("#divBtnAksiKomparasi").hide();

        var table = "";

        $.get("../todo/pre_interview/comparePelamar/"+arrApplicant, function(res){
            console.log(res);
            $("#divSpinner").hide();
            $("#divBtnAksiKomparasi").show();

            table += "<table class='table table-striped'>";
            table += "<thead>";
            table += "<tr>";
            table += "<th></th>";

            //Foto
            for (var i = 0; i < res.photo.length; i++) {
                table += "<th class='price-info'>\
                            <img src='../../storage/photo'/"+res.photo[i]+"' class='img-responsive' alt='Foto Pelamar'>\
                        </th>";
            }

            table += "</tr>";
            
            table += "</thead>";
            table += "<tbody>";

            //General information
            table += "<tr class='lblHeader' onclick='hideTR(1)'>";
            table += "<td></td><td colspan="+res.nama.length+">GENERAL INFORMATION <span class='pull-right'><i class='fa fa-chevron-down i1'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(1)'>";
            table += "<td colspan="+(res.nama.length + 1)+">\
                        GENERAL INFORMATION <span class='pull-right'><i class='fa fa-chevron-down i1'></i></span>\
                     </td>";
            table += "</tr>";

            //Nama
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Name</td>";
            table += "</tr>";

            table += "<tr class='trHide1'>";
            table += "<td>Name</td>";

            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>"+res.nama[i]+"</td>";
            }

            //Jenis Kelamin
            table += "</tr>";
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.jenis_kelamin.length+">Gender</td>";
            table += "</tr>";
            table += "<tr class='trHide1'>";
            table += "<td>Gender</td>";

            for (var i = 0; i < res.jenis_kelamin.length; i++) {
                table += '<td>'+res.jenis_kelamin[i]+'</td>';
            }

            //Marital status
            table += "</tr>";
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.status.length+">Marital Status</td>";
            table += "</tr>";
            table += "<tr class='trHide1'>";
            table += "<td>Marital Status</td>";

            for (var i = 0; i < res.status.length; i++) {
                table += '<td>'+res.status[i]+'</td>';
            }

            //Date of birth
            table += "</tr>";
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.tgl_lahir.length+">Date of Birth</td>";
            table += "</tr>";
            table += "<tr class='trHide1'>";
            table += "<td>Date of Birth</td>";

            for (var i = 0; i < res.tgl_lahir.length; i++) {
                table += '<td>'+res.tgl_lahir[i]+'</td>';
            }

            table += "</tr>";

            // =========================== END General information =================================

            //Educational Background
            table += "<tr class='lblHeader' onclick='hideTR(2)'>";
            table += "<td></td><td colspan="+res.edu.length+">EDUCATIONAL BACKGROUND <span class='pull-right'><i class='fa fa-chevron-down i2'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(2)'>";
            table += "<td colspan="+(res.edu.length + 1)+">\
                        EDUCATIONAL BACKGROUND <span class='pull-right'><i class='fa fa-chevron-down i2'></i></span>\
                     </td>";
            table += "</tr>";

            //Edu Level
            table += "<tr>";
            table += "<td></td><td colspan="+res.edu.length+">Edu Level</td>";
            table += "</tr>";

            table += "<tr class='trHide2'>";
            table += "<td>Edu Level</td>";
            
            for (var i = 0; i < res.edu.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if(res.edu[i].tingkat.length == 1)
                {
                    for (var j=0; j < res.edu[i].tingkat.length; j++) {
                        table += "<div class='col-md-12'>"+res.edu[i].tingkat[j]+"</div>";
                    }
                }
                else {
                    for (var j=0; j < res.edu[i].tingkat.length; j++) {
                        table += "<div class='col-md-6'>"+res.edu[i].tingkat[j]+"</div>";
                    }
                }                                
                    
                table += "</div>";
                table += "</td>";
            }
            
            table += "</tr>";

            //Institution Name
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.edu.length+">Institution Name</td>";
            table += "</tr>";
            table += "<tr class='trHide2'>";
            table += "<td>Institution Name</td>";
            for (var i = 0; i < res.edu.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if(res.edu[i].nama_institusi.length == 1)
                {
                    for (var j=0; j < res.edu[i].nama_institusi.length; j++) {
                        table += "<div class='col-md-12'>"+res.edu[i].nama_institusi[j]+"</div>";
                    }
                }
                else {
                    for (var j=0; j < res.edu[i].nama_institusi.length; j++) {
                        table += "<div class='col-md-6'>"+res.edu[i].nama_institusi[j]+"</div>";
                    } 
                }               
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            // //Major
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.edu.length+">Major</td>";
            table += "</tr>";
            table += "<tr class='trHide2'>";
            table += "<td>Major</td>";
            for (var i = 0; i < res.edu.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if(res.edu[i].major.length == 1)
                {
                    for (var j=0; j < res.edu[i].major.length; j++) {
                        table += "<div class='col-md-12'>"+res.edu[i].major[j]+"</div>";
                    } 
                }
                else {
                    for (var j=0; j < res.edu[i].major.length; j++) {
                        table += "<div class='col-md-6'>"+res.edu[i].major[j]+"</div>";
                    } 
                }               
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            // //GPA
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.edu.length+">GPA</td>";
            table += "</tr>";
            table += "<tr class='trHide2'>";
            table += "<td>GPA</td>";
            for (var i = 0; i < res.edu.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if(res.edu[i].gpa.length == 1)
                {
                    for (var j=0; j < res.edu[i].gpa.length; j++) {
                        table += "<div class='col-md-12'>"+ parseFloat(res.edu[i].gpa[j]).toFixed(2)+"</div>";
                    } 
                }
                else {
                    for (var j=0; j < res.edu[i].gpa.length; j++) {
                        table += "<div class='col-md-6'>"+ parseFloat(res.edu[i].gpa[j]).toFixed(2)+"</div>";
                    } 
                }               
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            // //Study Period
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.edu.length+">Study Period</td>";
            table += "</tr>";
            table += "<tr class='trHide2'>";
            table += "<td>Study Period</td>";
            for (var i = 0; i < res.edu.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if(res.edu[i].start_period.length == 1)
                {
                    for (var j=0; j < res.edu[i].start_period.length; j++) {
                        table += "<div class='col-md-12'>"+res.edu[i].start_period[j]+"</div>";
                    }  
                }
                else {
                    for (var j=0; j < res.edu[i].start_period.length; j++) {
                        table += "<div class='col-md-6'>"+res.edu[i].start_period[j]+"</div>";
                    }  
                }               
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            // =========================== END Edu Background =================================

            //Work Experience
            table += "<tr class='lblHeader' onclick='hideTR(4)'>";
            table += "<td></td><td colspan="+res.we.length+">WORK EXPERIENCE <span class='pull-right'><i class='fa fa-chevron-down i4'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(4)'>";
            table += "<td colspan="+(res.we.length + 1)+">\
                        WORK EXPERIENCE <span class='pull-right'><i class='fa fa-chevron-down i4'></i></span>\
                     </td>";
            table += "</tr>";

            //Company Name
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Company Name</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Company Name</td>";

            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].company.length; j++) {
                        table += "<div class='col-md-3'>"+res.we[i].company[j]+"</div>";
                    }
                }                                
                    
                table += "</div>";
                table += "</td>";
            }
            
            table += "</tr>";

            //Employment Period
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Employment Period</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Employment Period</td>";            

            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].start_period.length; j++) {
                        table += "<div class='col-md-3'>"+res.we[i].start_period[j]+"</div>";
                    } 
                }                             
                    
                table += "</div>";
                table += "</td>";
            }

            table += "</tr>";

            //Line of Business
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Line of Business</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Line of Business</td>";            

            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].bidang_usaha.length; j++) {
                        table += "<div class='col-md-3'>"+res.we[i].bidang_usaha[j]+"</div>";
                    } 
                }                              
                    
                table += "</div>";
                table += "</td>";
            }

            table += "</tr>";

            //Main Accountabilities
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Main Accountabilities</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Main Accountabilities</td>";
            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].main_accountability.length; j++) {
                        table += "<div class='col-md-3'>"+res.we[i].main_accountability[j]+"</div>";
                    }
                }                                 
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            //Achievement
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Achievement</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Achievement</td>";
            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].achievement.length; j++) {
                    table += "<div class='col-md-3'>"+res.we[i].achievement[j]+"</div>";
                }
                }                                
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            //Mistake
            table += "<tr>";
            table += "<td></td><td colspan="+res.we.length+">Mistake</td>";
            table += "</tr>";

            table += "<tr class='trHide4'>";
            table += "<td>Mistake</td>";
            for (var i = 0; i < res.we.length; i++) {

                table += "<td>";
                table += "<div class='row'>";

                if( res.we[i].length > 0){
                    for (var j=0; j < res.we[i].mistake.length; j++) {
                        table += "<div class='col-md-3'>"+res.we[i].mistake[j]+"</div>";
                    }
                }                                 
                    
                table += "</div>";
                table += "</td>";
            }
            table += "</tr>";

            // =========================== END Work Experience =================================

            //PSYCHOTEST RESULT
            table += "<tr class='lblHeader' onclick='hideTR(5)'>";
            table += "<td></td><td colspan="+res.nama.length+">PSYCHOTEST RESULT <span class='pull-right'><i class='fa fa-chevron-down i5'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(5)'>";
            table += "<td colspan="+(res.nama.length + 1)+">\
                        PSYCHOTEST RESULT <span class='pull-right'><i class='fa fa-chevron-down i5'></i></span>\
                     </td>";
            table += "</tr>";

            //NTM
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">NTM</td>";
            table += "</tr>";

            table += "<tr class='trHide5'>";
            table += "<td>NTM</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //(list test yang ada di psikotest)
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">(list test yang ada di psikotest)</td>";
            table += "</tr>";

            table += "<tr class='trHide5'>";
            table += "<td>(list test yang ada di psikotest)</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //DISC
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">DISC</td>";
            table += "</tr>";

            table += "<tr class='trHide5'>";
            table += "<td>DISC</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Pengenalan Diri
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Pengenalan Diri</td>";
            table += "</tr>";

            table += "<tr class='trHide5'>";
            table += "<td>Pengenalan Diri</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Complete CV
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Complete CV</td>";
            table += "</tr>";

            table += "<tr class='trHide5'>";
            table += "<td>Complete CV</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            // =========================== END PSYCHOTEST RESULT =================================

            //JOB INTEREST
            table += "<tr class='lblHeader' onclick='hideTR(6)'>";
            table += "<td></td><td colspan="+res.nama.length+">JOB INTEREST <span class='pull-right'><i class='fa fa-chevron-down i6'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(6)'>";
            table += "<td colspan="+(res.nama.length + 1)+">\
                        JOB INTEREST <span class='pull-right'><i class='fa fa-chevron-down i6'></i></span>\
                     </td>";
            table += "</tr>";

            //Line of Business
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Line of Business</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Line of Business</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Work Satisfaction
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Work Satisfaction</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Work Satisfaction</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Facility Expectation
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Facility Expectation</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Facility Expectation</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Salary Expectation
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Salary Expectation</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Salary Expectation</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Strenghs
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Strenghs</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Strenghs</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            //Weakness
            table += "<tr>";
            table += "<td></td><td colspan="+res.nama.length+">Weakness</td>";
            table += "</tr>";

            table += "<tr class='trHide6'>";
            table += "<td>Weakness</td>";
            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }
            table += "</tr>";

            // =========================== END JOB INTEREST =================================

            //SKILLS
            table += "<tr class='lblHeader' onclick='hideTR(3)'>";
            table += "<td></td><td colspan="+res.nama.length+">SKILLS <span class='pull-right'><i class='fa fa-chevron-down i3'></i></span></td>";
            table += "</tr>";
            
            table += "<tr class='lblHeader' onclick='hideTR(3)'>";
            table += "<td colspan="+(res.nama.length + 1)+">\
                        SKILLS <span class='pull-right'><i class='fa fa-chevron-down i3'></i></span>\
                    </td>";
            table += "</tr>";

            //Skill Name
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.nama.length+">Skill Name</td>";
            table += "</tr>";
            table += "<tr class='trHide3'>";
            table += "<td>Skill Name</td>";

            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }

            //Score
            table += "</tr>";
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.nama.length+">Score</td>";
            table += "</tr>";
            table += "<tr class='trHide3'>";
            table += "<td>Score</td>";

            for (var i = 0; i < res.nama.length; i++) {
                table += "<td>-</td>";
            }

            //Pilihan checkbox
            table += "</tr>";
            table += "<tr>";
            table += "<td>&nbsp;</td><td colspan="+res.no_applicant.length+">Interview</td>";
            table += "</tr>";
            table += "<tr class='trHide3'>";
            table += "<td>Interview</td>";

            for (var i = 0; i < res.no_applicant.length; i++) {
                table += '<td><input type="checkbox" name="no_applicant_komparasi[]" value="'+res.no_applicant[i]+'"></td>';
            }

            table += "</tr>";
            table += "</tbody></table>";
            
            $("#divComparison").append(table);
        });

        $('#modalComparePelamar').modal('show');
    }

    function kirimPelamar(tipe)
    {
        var len = $("[name='no_applicant_komparasi[]']:checked").length;

        if(len > 0)
        {
            swal({
                title: "Perhatian!",
                text: 'Anda memilih '+tipe+', lanjutkan ? ',
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#EF5350",
                confirmButtonText: "Lanjutkan",
                cancelButtonText: "Batal",
                closeOnConfirm: false,
                closeOnCancel: true,
                html: true
            },
            function(isConfirm){
                if (isConfirm) {

                    $("#txt_tipe_submit").val(tipe);
                    $("#frmKomparasi").submit();                    
                }
            });

        }else {

            swal({
                title: "Perhatian!",
                text: "Pelamar belum ada yang dipilih!",
                type: "warning",
                confirmButtonColor: "#EF5350",
                confirmButtonText: "OK",
                cancelButtonText: "Batal"
            });
            return false;
        }
        
    }

    function hideTR(no)
    {
        $(".trHide"+no).toggle();
        $(".trHide"+no+", .i"+no).toggleClass("fa-chevron-left fa-chevron-down");
    }