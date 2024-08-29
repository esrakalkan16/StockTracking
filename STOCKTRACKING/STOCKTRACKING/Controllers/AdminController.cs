using STOCKTRACKING.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using STOCKTRACKING.Models.KategoryModels;
using STOCKTRACKING.Models.UrunModels;
using STOCKTRACKING.Models.MusteriModels;
using System.Net;


namespace STOCKTRACKING.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        StokSatisProgramıEntities db = new StokSatisProgramıEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Musteriler()
        {
            var degerler = db.ACT.ToList();
            return View("Musteriler/ACT", degerler);
        }
        public ActionResult Kategoriler()
        {
            var degerler = db.CATEGORY.ToList();
            return View("Kategoriler/CATEGORY", degerler);
        }
        public ActionResult Stok()
        {
            var degerler = db.STOCK.ToList();
            return View("Stok/STOCK", degerler);
        }

        //kategori sil ekle düzenle 

        [HttpGet]
        public ActionResult KategoriForm(int? CATEGORYID)
        {
            KategoryFormModel model = new KategoryFormModel();
            model.CATEGORY = db.CATEGORY.Where(m => m.CATEGORYID == CATEGORYID).FirstOrDefault();
            return View("Kategoriler/KategoriForm", model);
        }
        [HttpPost]
        public ActionResult KategoriForm(KategoryFormModel model)
        {
            if (model.CATEGORY.CATEGORYID != 0)
            {

                var yeni = db.CATEGORY.Find(model.CATEGORY.CATEGORYID);
                yeni.CATEGORYNAME = model.CATEGORY.CATEGORYNAME;
                db.SaveChanges();

            }
            else
            {

                // Tablodaki en yüksek CATEGORYID değerini al
                var maxCategoryId = db.CATEGORY.Max(m => m.CATEGORYID);

                // Yeni CATEGORYID'yi en yüksek ID'nin bir fazlası olarak ayarla/boş gelme durumunda düsün
                model.CATEGORY.CATEGORYID = db.CATEGORY.Select(m => m.CATEGORYID).DefaultIfEmpty(0).Max() + 1;
                db.CATEGORY.Add(model.CATEGORY);
                db.SaveChanges();

            }
            return RedirectToAction("Kategoriler");
        }
        [HttpPost]
        public ActionResult SIL(int CATEGORYID)
        {
            var category = db.CATEGORY.Where(m => m.CATEGORYID == CATEGORYID).FirstOrDefault();
            if (category != null)
            {
                var relatedStockCategory = db.STOCKCATEGORYS.Where(m => m.CATEGORYID == CATEGORYID).FirstOrDefault();
                if (relatedStockCategory != null)
                {
                    return Json(new { success = false, message = "Bu kategoriye bağlı stoklar var, silemezsiniz." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.CATEGORY.Remove(category);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Silme işlemi başarılı." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, message = "Silinecek kategori bulunamadı." }, JsonRequestBehavior.AllowGet);
        }


            //stok sil ekle  düzenle

            [HttpGet]
        public ActionResult UrunForm(int? STOCKID)
        {
            UrunFormModel model = new UrunFormModel();
            model.STOCK = db.STOCK.Where(m => m.STOCKID == STOCKID).FirstOrDefault();
            model.CATEGORY = db.CATEGORY.ToList();
            model.STOCKCATEGORYS = db.STOCKCATEGORYS.Where(sc => sc.STOCKID == STOCKID).ToList();
            return View("Stok/UrunForm", model);
        }
        [HttpPost]
        public ActionResult UrunForm(UrunFormModel model, string[] esra)
        {
            if (model.STOCK.STOCKID != 0)
            {
                var yeni = db.STOCK.Find(model.STOCK.STOCKID);
                yeni.STOCKNAME = model.STOCK.STOCKNAME;
                yeni.STOCKNO = model.STOCK.STOCKNO;
                yeni.SALEPRİCE = model.STOCK.SALEPRİCE;
                yeni.PURCHASEPRİCE = model.STOCK.PURCHASEPRİCE;

                // Mevcut kategorileri al ve sil
                var mevcutKategoriler = db.STOCKCATEGORYS.Where(sc => sc.STOCKID == yeni.STOCKID).ToList();
                db.STOCKCATEGORYS.RemoveRange(mevcutKategoriler);
                db.SaveChanges();

                // Yeni seçilen kategorileri ekle
                foreach (var kategoriId in esra)
                {
                    var stokKategori = new STOCKCATEGORYS
                    {
                        STOCKID = yeni.STOCKID,
                        CATEGORYID = int.Parse(kategoriId)
                    };
                    db.STOCKCATEGORYS.Add(stokKategori);
                }

                db.SaveChanges();
            }
            else
            {
                // Yeni stok ekleme işlemi
                model.STOCK.STOCKID = db.STOCK.Select(m => m.STOCKID).DefaultIfEmpty(0).Max() + 1;
                db.STOCK.Add(model.STOCK);
                db.SaveChanges();

                // Yeni kategorileri ekle
                foreach (var kategoriId in esra)
                {
                    var stokKategori = new STOCKCATEGORYS
                    {
                        STOCKID = model.STOCK.STOCKID,
                        CATEGORYID = int.Parse(kategoriId)
                    };
                    db.STOCKCATEGORYS.Add(stokKategori);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Stok", model);
        }

        public ActionResult SIL2(int STOCKID)
        {
            var stok = db.STOCK.Find(STOCKID);
            if (stok != null )
            {
                db.STOCK.Remove(stok);
                db.SaveChanges();
            }
            return RedirectToAction("Stok");
        }

        //musteri sil ekle düzenle

        [HttpGet]
        public ActionResult MusteriForm(int? ACTID)
        {
            MusteriFormModel model = new MusteriFormModel();
            model.ACT = db.ACT.Where(m => m.ACTID == ACTID).FirstOrDefault();
            return View("Musteriler/MusteriForm", model);
        }
        [HttpPost]
        public ActionResult MusteriForm(MusteriFormModel model)
        {
            if (model.ACT.ACTID != 0)
            {
                var yeni = db.ACT.Find(model.ACT.ACTID);
                yeni.ACTNAME = model.ACT.ACTNAME;
                yeni.ACTNO = model.ACT.ACTNO;
                yeni.EMAİL = model.ACT.EMAİL;
                yeni.PHONE1 = model.ACT.PHONE1;
                db.SaveChanges();
            }
            else
            {


                // Tablodaki en yüksek STOCKID değerini al
                var maxCategoryId = db.STOCK.Max(m => m.STOCKID);

                // Yeni CATEGORYID'yi en yüksek ID'nin bir fazlası olarak ayarla/boş gelme durumunda düsün
                model.ACT.ACTID = db.ACT.Select(m => m.ACTID).DefaultIfEmpty(0).Max() + 1;
                db.ACT.Add(model.ACT);
                db.SaveChanges();

            }
            return RedirectToAction("Musteriler");
        }
        public ActionResult SIL3(int ACTID)
        {
            var musteri = db.ACT.Find(ACTID);
            if (musteri != null)
            {
                db.ACT.Remove(musteri);
                db.SaveChanges();
            }

            return RedirectToAction("Musteriler");
        }

    }
}
