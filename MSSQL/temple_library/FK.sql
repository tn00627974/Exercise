Alter Table Books

ADD Foreign key (Pub_ID) References publishers(Pub_ID);

Alter Table Records

ADD Foreign key (Book_ID) References Books(Book_ID);

Alter Table Records

ADD Foreign key (Guest_ID) References Guests(Guest_ID);

