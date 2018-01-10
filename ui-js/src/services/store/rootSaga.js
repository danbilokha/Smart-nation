import { all } from 'redux-saga/effects';

import searchBooks from '../../containers/page/home/search/search.saga';
import loginProccess from '../../containers/page/login/login.saga';
import registrationProcess from '../../containers/page/register/register.saga';
import cabinetProcess from '../../containers/page/cabinet/cabinet.saga';
import addingBookProcess from '../../containers/page/cabinet/addBook/addBookForm/addBook.saga';
import fetchingBookShelfBooks from '../../containers/page/cabinet/bookshelf/bookshelf.saga';
import fetchingOrderBookData from '../../containers/book/orderBook/orderBook.saga';
import watchFetchingBook from '../../containers/page/book/book.saga';

export default function* rootSaga() {
    yield all([
        searchBooks(),
        loginProccess(),
        registrationProcess(),
        cabinetProcess(),
        addingBookProcess(),
        fetchingBookShelfBooks(),
        fetchingOrderBookData(),
        watchFetchingBook(),
    ]);
}
