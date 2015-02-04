package lab660.lab1;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.sql.Connection;
import java.sql.Date;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Map;
import java.util.TreeMap;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

public class LectureBD {
	
	private Connection dbConnection = null;

	public enum Province {
		AB, BC, MB, NB,
		NL, NS, ON, PE, QC, SK, NT
	}
	
	private int nextLanguageId = 0;
	private int nextCountryId = 0;
	
	public enum CreditCardType
	{
		MasterCard,
		Visa,
		AmericanExpress
	}
	
	public class Role {
		public Role(int i, String n, String p) {
			id = i;
			nom = n;
			personnage = p;
		}

		protected int id;
		protected String nom;
		protected String personnage;
	}

	public LectureBD() {
		connectionBD();
	}

	public void lecturePersonnes(String nomFichier) {
		try {
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			XmlPullParser parser = factory.newPullParser();

			InputStream is = new FileInputStream(nomFichier);
			parser.setInput(is, null);

			int eventType = parser.getEventType();

			String tag = null, nom = null, anniversaire = null, lieu = null, photo = null, bio = null;

			int id = -1;

			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					tag = parser.getName();

					if (tag.equals("personne")
							&& parser.getAttributeCount() == 1)
						id = Integer.parseInt(parser.getAttributeValue(0));
				} else if (eventType == XmlPullParser.END_TAG) {
					tag = null;

					if (parser.getName().equals("personne") && id >= 0) {
						insertionPersonne(id, nom, anniversaire, lieu, photo,
								bio);

						id = -1;
						nom = null;
						anniversaire = null;
						lieu = null;
						photo = null;
						bio = null;
					}
				} else if (eventType == XmlPullParser.TEXT && id >= 0) {
					if (tag != null) {
						if (tag.equals("nom"))
							nom = parser.getText();
						else if (tag.equals("anniversaire"))
							anniversaire = parser.getText();
						else if (tag.equals("lieu"))
							lieu = parser.getText();
						else if (tag.equals("photo"))
							photo = parser.getText();
						else if (tag.equals("bio"))
							bio = parser.getText();
					}
				}

				eventType = parser.next();
			}
		} catch (XmlPullParserException e) {
			System.out.println(e);
		} catch (IOException e) {
			System.out.println("IOException while parsing " + nomFichier);
		}
	}

	public void lectureFilms(String nomFichier) {
		try {
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			XmlPullParser parser = factory.newPullParser();

			InputStream is = new FileInputStream(nomFichier);
			parser.setInput(is, null);

			int eventType = parser.getEventType();

			String tag = null, titre = null, langue = null, poster = null, roleNom = null, rolePersonnage = null, realisateurNom = null, resume = null;

			ArrayList<String> pays = new ArrayList<String>();
			ArrayList<String> genres = new ArrayList<String>();
			ArrayList<String> scenaristes = new ArrayList<String>();
			ArrayList<Role> roles = new ArrayList<Role>();
			ArrayList<String> annonces = new ArrayList<String>();

			int id = -1, annee = -1, duree = -1, roleId = -1, realisateurId = -1;

			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					tag = parser.getName();

					if (tag.equals("film") && parser.getAttributeCount() == 1)
						id = Integer.parseInt(parser.getAttributeValue(0));
					else if (tag.equals("realisateur")
							&& parser.getAttributeCount() == 1)
						realisateurId = Integer.parseInt(parser
								.getAttributeValue(0));
					else if (tag.equals("acteur")
							&& parser.getAttributeCount() == 1)
						roleId = Integer.parseInt(parser.getAttributeValue(0));
				} else if (eventType == XmlPullParser.END_TAG) {
					tag = null;

					if (parser.getName().equals("film") && id >= 0) {
						insertionFilm(id, titre, annee, pays, langue, duree,
								resume, genres, realisateurNom, realisateurId,
								scenaristes, roles, poster, annonces);

						id = -1;
						annee = -1;
						duree = -1;
						titre = null;
						langue = null;
						poster = null;
						resume = null;
						realisateurNom = null;
						roleNom = null;
						rolePersonnage = null;
						realisateurId = -1;
						roleId = -1;

						genres.clear();
						scenaristes.clear();
						roles.clear();
						annonces.clear();
						pays.clear();
					}
					if (parser.getName().equals("role") && roleId >= 0) {
						roles.add(new Role(roleId, roleNom, rolePersonnage));
						roleId = -1;
						roleNom = null;
						rolePersonnage = null;
					}
				} else if (eventType == XmlPullParser.TEXT && id >= 0) {
					if (tag != null) {
						if (tag.equals("titre"))
							titre = parser.getText();
						else if (tag.equals("annee"))
							annee = Integer.parseInt(parser.getText());
						else if (tag.equals("pays"))
							pays.add(parser.getText());
						else if (tag.equals("langue"))
							langue = parser.getText();
						else if (tag.equals("duree"))
							duree = Integer.parseInt(parser.getText());
						else if (tag.equals("resume"))
							resume = parser.getText();
						else if (tag.equals("genre"))
							genres.add(parser.getText());
						else if (tag.equals("realisateur"))
							realisateurNom = parser.getText();
						else if (tag.equals("scenariste"))
							scenaristes.add(parser.getText());
						else if (tag.equals("acteur"))
							roleNom = parser.getText();
						else if (tag.equals("personnage"))
							rolePersonnage = parser.getText();
						else if (tag.equals("poster"))
							poster = parser.getText();
						else if (tag.equals("annonce"))
							annonces.add(parser.getText());
					}
				}

				eventType = parser.next();
			}
		} catch (XmlPullParserException e) {
			System.out.println(e);
		} catch (IOException e) {
			System.out.println("IOException while parsing " + nomFichier);
		}
	}

	public void closeConnection () {
		if (dbConnection != null) {
			try {
				dbConnection.close();
			} catch (SQLException e) {
				// TODO Bloc catch généré automatiquement
				e.printStackTrace();
			}
		}
	}
	
	public void lectureClients(String nomFichier) {
		try {
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			XmlPullParser parser = factory.newPullParser();

			InputStream is = new FileInputStream(nomFichier);
			parser.setInput(is, null);

			int eventType = parser.getEventType();

			String tag = null, nomFamille = null, prenom = null, courriel = null, tel = null, anniv = null, adresse = null, ville = null, province = null, codePostal = null, carte = null, noCarte = null, motDePasse = null, forfait = null;

			int id = -1, expMois = -1, expAnnee = -1;

			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					tag = parser.getName();

					if (tag.equals("client") && parser.getAttributeCount() == 1)
						id = Integer.parseInt(parser.getAttributeValue(0));
				} else if (eventType == XmlPullParser.END_TAG) {
					tag = null;

					if (parser.getName().equals("client") && id >= 0) {
						insertionClient(id, nomFamille, prenom, courriel, tel,
								anniv, adresse, ville, province, codePostal,
								carte, noCarte, expMois, expAnnee, motDePasse,
								forfait);

						nomFamille = null;
						prenom = null;
						courriel = null;
						tel = null;
						anniv = null;
						adresse = null;
						ville = null;
						province = null;
						codePostal = null;
						carte = null;
						noCarte = null;
						motDePasse = null;
						forfait = null;

						id = -1;
						expMois = -1;
						expAnnee = -1;
					}
				} else if (eventType == XmlPullParser.TEXT && id >= 0) {
					if (tag != null) {
						if (tag.equals("nom-famille"))
							nomFamille = parser.getText();
						else if (tag.equals("prenom"))
							prenom = parser.getText();
						else if (tag.equals("courriel"))
							courriel = parser.getText();
						else if (tag.equals("tel"))
							tel = parser.getText();
						else if (tag.equals("anniversaire"))
							anniv = parser.getText();
						else if (tag.equals("adresse"))
							adresse = parser.getText();
						else if (tag.equals("ville"))
							ville = parser.getText();
						else if (tag.equals("province"))
							province = parser.getText();
						else if (tag.equals("code-postal"))
							codePostal = parser.getText();
						else if (tag.equals("carte"))
							carte = parser.getText();
						else if (tag.equals("no"))
							noCarte = parser.getText();
						else if (tag.equals("exp-mois"))
							expMois = Integer.parseInt(parser.getText());
						else if (tag.equals("exp-annee"))
							expAnnee = Integer.parseInt(parser.getText());
						else if (tag.equals("mot-de-passe"))
							motDePasse = parser.getText();
						else if (tag.equals("forfait"))
							forfait = parser.getText();
					}
				}

				eventType = parser.next();
			}
		} catch (XmlPullParserException e) {
			System.out.println(e);
		} catch (IOException e) {
			System.out.println("IOException while parsing " + nomFichier);
		}
	}

	private void insertionPersonne(int id, String nom, String anniv,
			String lieu, String photo, String bio) {
		
		
		String biography = bio;
		
		if (biography != null)
			biography = bio.substring(0, Math.min(bio.length(), 3000));
		
		String names[] = nom.split(" ", 2);
				
		String lastName = "";
		String firstName = null;
		
		firstName = names[0];
		
		if (names.length == 2)
		{
			lastName = names[1];		
		}
		
		DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
		
		Date birthDate = null;
		if (anniv != null && !anniv.equals(""))
		try {
			birthDate = new java.sql.Date(format.parse(anniv).getTime());
		} catch (ParseException e1) {
			// TODO Bloc catch généré automatiquement
			e1.printStackTrace();
		}
		
		String insertTableSQL = "INSERT INTO professionals"
				+ "(id, first_name, last_name, birthdate, birthplace, biography) VALUES"
				+ "(?,?,?,?,?,?)";
		
		PreparedStatement preparedStatement = null;
		try {
		preparedStatement = dbConnection.prepareStatement(insertTableSQL);
		preparedStatement.setInt(1, id);
		preparedStatement.setString(2, firstName);
		preparedStatement.setString(3, lastName);
		preparedStatement.setDate(4, birthDate);
		preparedStatement.setString(5, lieu);
		preparedStatement.setString(6, biography);
		// execute insert SQL stetement
		preparedStatement.executeUpdate();
		}
		catch (SQLException e) {
			 
			System.out.println(e.getMessage());
		} finally {
			if (preparedStatement != null) {
				try {
					preparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
			}
		}
	}

	private void insertionFilm(int id, String titre, int annee,
			ArrayList<String> pays, String langue, int duree, String resume,
			ArrayList<String> genres, String realisateurNom, int realisateurId,
			ArrayList<String> scenaristes, ArrayList<Role> roles,
			String poster, ArrayList<String> annonces) {
			
		PreparedStatement queryPreparedStatement = null;
		PreparedStatement preparedStatement = null;
		String select = null;
		ResultSet result = null;
		String insertTableSQL = null;
		
		int id_language = 0;
		try {
		
		select = "select id from languages where name=?";
		queryPreparedStatement = dbConnection.prepareStatement(select);
		queryPreparedStatement.setString(1, langue);
		result = queryPreparedStatement.executeQuery();
		if (!result.next())
		{
			insertTableSQL = "INSERT INTO languages"
					+ "(id, name) VALUES"
					+ "(?,?)";
			
			preparedStatement = null;
			try {
			preparedStatement = dbConnection.prepareStatement(insertTableSQL);
			preparedStatement.setInt(1, ++nextLanguageId);
			id_language = nextLanguageId;
			preparedStatement.setString(2, langue);
			System.out.println(insertTableSQL + " id " + nextLanguageId + " langue " + langue);
			preparedStatement.executeUpdate();
			}
			catch (SQLException e) {
				 
				System.out.println(e.getMessage());
			} finally {
				if (preparedStatement != null) {
					try {
						preparedStatement.close();
						queryPreparedStatement.close();
					} catch (SQLException e) {
						// TODO Bloc catch généré automatiquement
						e.printStackTrace();
					}
				}
			}
		}
		else
		{
			id_language = result.getInt("id");
			queryPreparedStatement.close();
		}
		
		insertTableSQL = "INSERT INTO films"
				+ "(id, title, year, number_of_copies, summary, length_in_minutes, "
				+ "id_director, id_language) VALUES"
				+ "(?,?,?,?,?,?,?,?)";
		
		
		
		preparedStatement = dbConnection.prepareStatement(insertTableSQL);
		preparedStatement.setInt(1, id);
		preparedStatement.setString(2, titre);
		// TODO: randomize the number
		
		preparedStatement.setInt(3, annee);
		preparedStatement.setInt(4, 1 + (int)(Math.random()*100));
		preparedStatement.setString(5, resume);
		preparedStatement.setInt(6, duree);
		preparedStatement.setInt(7, realisateurId);
		preparedStatement.setInt(8, id_language);
		System.out.println(insertTableSQL + " id " + id + " id_language " + id_language + " id_realisation " + realisateurId);
		preparedStatement.executeUpdate();
		}
		catch (SQLException e) {
			 
			System.out.println(e.getMessage());
		} finally {
			if (preparedStatement != null) {
				try {
					preparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
			}
		}
		
		
		for (String currentCountry: pays)
		{
			
			select = "select id from countries where name=?";
			try {
				queryPreparedStatement = dbConnection.prepareStatement(select);
				queryPreparedStatement.setString(1, currentCountry);
				result = queryPreparedStatement.executeQuery();
			} catch (SQLException e1) {
				// TODO Bloc catch généré automatiquement
				try {
					queryPreparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
				e1.printStackTrace();
			}

			
			int id_country = 0;
			
			try {
				if (!result.next())
				{
					insertTableSQL = "INSERT INTO countries"
							+ "(id, name) VALUES"
							+ "(?,?)";
					preparedStatement = null;
					try {
					preparedStatement = dbConnection.prepareStatement(insertTableSQL);
					preparedStatement.setInt(1, ++nextCountryId);
					id_country = nextCountryId;
					preparedStatement.setString(2, currentCountry);
					System.out.println(insertTableSQL + " id " + nextCountryId + " name " + currentCountry);
					preparedStatement.executeUpdate();
					}
					catch (SQLException e) {
						 
						System.out.println(e.getMessage());
					} finally {
						if (preparedStatement != null) {
							try {
								preparedStatement.close();
							} catch (SQLException e) {
								// TODO Bloc catch généré automatiquement
								e.printStackTrace();
							}
						}
					}
				}
				else
				{
					try {
						id_country = result.getInt("id");
					} catch (SQLException e) {
						// TODO Bloc catch généré automatiquement
						e.printStackTrace();
					}
				}
				
			} catch (SQLException e1) {
				// TODO Bloc catch généré automatiquement
				e1.printStackTrace();
			}
			finally
			{
				try {
					queryPreparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
			}
			
			insertTableSQL = "INSERT INTO films_countries"
					+ "(id_film, id_country) VALUES"
					+ "(?,?)";
			
			preparedStatement = null;
			try {
			preparedStatement = dbConnection.prepareStatement(insertTableSQL);
			preparedStatement.setInt(1, id);
			preparedStatement.setInt(2, id_country);
			
			System.out.println(insertTableSQL + " id_film " + id + " id_country " + id_country);
			preparedStatement.executeUpdate();
			}
			catch (SQLException e) {
				 
				System.out.println(e.getMessage());
			} finally {
				if (preparedStatement != null) {
					try {
						preparedStatement.close();
					} catch (SQLException e) {
						// TODO Bloc catch généré automatiquement
						e.printStackTrace();
					}
				}
			}
			
		}
		
				
		for (Role role: roles)
		{
			
			
			select = "select id from professionals where first_name || ' ' || last_name=?";
			int id_professional = 0;
			try {
				queryPreparedStatement = dbConnection.prepareStatement(select);
				queryPreparedStatement.setString(1, role.nom);
				result = queryPreparedStatement.executeQuery();
				
				if (result.next())
					id_professional = result.getInt("id");
					
				
			} catch (SQLException e1) {
				// TODO Bloc catch généré automatiquement
				e1.printStackTrace();
			} finally {
				try {
					queryPreparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
			}
			
			insertTableSQL = "INSERT INTO films_roles"
					+ "(id, id_film, id_professional, character) VALUES"
					+ "(?,?,?,?)";
			
			preparedStatement = null;
			try {
			preparedStatement = dbConnection.prepareStatement(insertTableSQL);
			preparedStatement.setInt(1, role.id);
			preparedStatement.setInt(2, id);
			preparedStatement.setInt(3, id_professional);
			preparedStatement.setString(4, role.personnage);
			preparedStatement.executeUpdate();
			}
			catch (SQLException e) {
				 
				System.out.println(e.getMessage());
			} finally {
				if (preparedStatement != null) {
					try {
						preparedStatement.close();
					} catch (SQLException e) {
						// TODO Bloc catch généré automatiquement
						e.printStackTrace();
					}
				}
			}
		}
		
		
		
		// On le film dans la BD
	}

	private void insertionClient(int id, String nomFamille, String prenom,
			String courriel, String tel, String anniv, String adresse,
			String ville, String province, String codePostal, String carte,
			String noCarte, int expMois, int expAnnee, String motDePasse,
			String forfait) {
		
		DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
		
		Date birthDate = null;
		if (anniv != null && !anniv.equals(""))
		try {
			birthDate = new java.sql.Date(format.parse(anniv).getTime());
		} catch (ParseException e1) {
			// TODO Bloc catch généré automatiquement
			e1.printStackTrace();
		}
		
		String address[] = adresse.split(" ", 2);
		String civicNumber = address[0];
		String address_street = address[1];
		
		int provinceNumber = Province.valueOf(province).ordinal();

		String insertTableSQL = "INSERT INTO persons"
				+ "(id, first_name, last_name, email, telephone, address_civic_number, "
				+ "address_street, address_city, address_province, address_postal_code, birthdate, password) VALUES"
				+ "(?,?,?,?,?,?,?,?,?,?,?,?)";
		
		if (!codePostal.equals(""))
			codePostal = codePostal.replace(" ", "");
		
		PreparedStatement preparedStatement = null;
		try {
		preparedStatement = dbConnection.prepareStatement(insertTableSQL);
		preparedStatement.setInt(1, id);
		preparedStatement.setString(2, prenom);
		preparedStatement.setString(3, nomFamille);
		preparedStatement.setString(4, courriel);
		preparedStatement.setString(5, tel);
		preparedStatement.setString(6, civicNumber);
		preparedStatement.setString(7, address_street);
		preparedStatement.setString(8, ville);
		preparedStatement.setInt(9, provinceNumber);
		preparedStatement.setString(10, codePostal);		
		preparedStatement.setDate(11, birthDate);
		preparedStatement.setString(12, motDePasse);
		// execute insert SQL stetement
		preparedStatement.executeUpdate();
		}
		catch (SQLException e) {
			 
			System.out.println(e.getMessage());
		} finally {
			if (preparedStatement != null) {
				try {
					preparedStatement.close();
				} catch (SQLException e) {
					// TODO Bloc catch généré automatiquement
					e.printStackTrace();
				}
			}
		}
		
		int idPackage = 0;
		
		if (forfait.equals("D"))
			idPackage = 1;
		else if (forfait.equals("I"))
			idPackage = 2;
		else if (forfait.equals("A"))
			idPackage = 3;
		
		insertTableSQL = "INSERT INTO customers"
				+ "(id, credit_card_number, credit_card_type, credit_card_expiration_month, "
				+ "credit_card_expiration_year, id_package) VALUES"
				+ "(?,?,?,?,?,?)";
		try {
			preparedStatement = dbConnection.prepareStatement(insertTableSQL);
			preparedStatement.setInt(1, id);
			preparedStatement.setString(2, noCarte);
			preparedStatement.setInt(3, CreditCardType.valueOf(carte).ordinal());
			preparedStatement.setInt(4, expMois);
			preparedStatement.setInt(5, expAnnee);
			preparedStatement.setInt(6, idPackage);
			// execute insert SQL stetement
			preparedStatement.executeUpdate();
			}
			catch (SQLException e) {
				 
				System.out.println(e.getMessage());
			} finally {
				if (preparedStatement != null) {
					try {
						preparedStatement.close();
					} catch (SQLException e) {
						// TODO Bloc catch généré automatiquement
						e.printStackTrace();
					}
				}
			}
		
		// On le client dans la BD
	}

	private void connectionBD() {
		System.out.println("-------- Oracle JDBC Connection Testing ------");

		try {

			Class.forName("oracle.jdbc.driver.OracleDriver");

		} catch (ClassNotFoundException e) {

			System.out.println("Where is your Oracle JDBC Driver?");
			e.printStackTrace();
			return;

		}

		System.out.println("Oracle JDBC Driver Registered!");

		try {

			dbConnection = DriverManager.getConnection(
					"jdbc:oracle:thin:@big-data-3.logti.etsmtl.ca:1521:LOG660", "equipe23",
					"Jf51vmZi");

		} catch (SQLException e) {

			System.out.println("Connection Failed! Check output console");
			e.printStackTrace();
			return;

		}

		if (dbConnection != null) {
			System.out.println("You made it, take control your database now!");
		} else {
			System.out.println("Failed to make connection!");
		}
	}

	public static void main(String[] args) {
		LectureBD lecture = new LectureBD();

		lecture.lecturePersonnes(args[0]);
		lecture.lectureFilms(args[1]);
		lecture.lectureClients(args[2]);
		lecture.closeConnection();
	}
}
